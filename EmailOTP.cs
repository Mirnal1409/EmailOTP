using System.Collections.Concurrent;
using System.Text.RegularExpressions;


public class EmailOTP
{
    private const string Domain = ".dso.org.sg";
    private const int MaxAttempts = 10;
    private const int OtpValiditySeconds = 60;
    private readonly ConcurrentDictionary<string, (string Otp, DateTime ExpiryTime)> _otpStore = new();

    public void Start()
    {
        Console.WriteLine("Email OTP Module Initialized.");
    }

    public void Close()
    {
        Console.WriteLine("Email OTP Module Shut Down.");
    }

    private string GenerateOtp()
    {
        var random = new Random();
        return random.Next(0, 999999).ToString("D6"); // Generate a 6-digit OTP
    }

    public int GenerateOtpEmail(string userEmail)
    {
        if (!Regex.IsMatch(userEmail, @"^[a-zA-Z0-9._%+-]+@([a-zA-Z0-9-]+\.)*dso\.org\.sg$"))
        {
             Console.WriteLine("Email validation failed. Only emails from '.dso.org.sg' are allowed.");
            return StatusConstants.StatusEmailInvalid;
        }
        string otp = GenerateOtp();
        Console.WriteLine("OTP:",otp);
        DateTime expiryTime = DateTime.UtcNow.AddSeconds(OtpValiditySeconds);

        _otpStore[userEmail] = (otp, expiryTime);

        try
        {
            string emailBody = $"Your OTP Code is {otp}. The code is valid for 1 minute.";
            SendEmail(userEmail, emailBody);
            return StatusConstants.StatusEmailOk;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            return StatusConstants.StatusEmailFail;
        }
    }

    public int CheckOtp(IInputStream input, string userEmail)
{
    Console.WriteLine($"Waiting for OTP input for email: {userEmail}");

    if (!_otpStore.ContainsKey(userEmail))
    {
        Console.WriteLine("No OTP found for this email.");
        return StatusConstants.StatusOtpFail;
    }

    var (currentOtp, expiryTime) = _otpStore[userEmail];
    if (DateTime.UtcNow > expiryTime)
    {
        Console.WriteLine("OTP has expired.");
        return StatusConstants.StatusOtpTimeout;
    }

    int maxAttempts = 10;
    int attempts = 0;

    // Timeout after 1 minute
    CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

    try
    {
        while (attempts < maxAttempts && !cts.Token.IsCancellationRequested)
        {
            Console.WriteLine($"Attempt {attempts + 1}/{maxAttempts}: Please enter the OTP.");
            string userOtp;

            try
            {
                userOtp = input.ReadOtp(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Timeout occurred while waiting for OTP.");
                return StatusConstants.StatusOtpTimeout;
            }

            if (userOtp == currentOtp)
            {
                Console.WriteLine("OTP verified successfully!");
                return StatusConstants.StatusOtpOk;
            }

            Console.WriteLine("Incorrect OTP. Try again.");
            attempts++;
        }

        return attempts >= maxAttempts
            ? StatusConstants.StatusOtpFail
            : StatusConstants.StatusOtpTimeout;
    }
    finally
    {
        cts.Dispose();
    }
}


    private void SendEmail(string emailAddress, string emailBody)
    {
        Console.WriteLine($"Sending email to {emailAddress}:\n{emailBody}");
    }
}
