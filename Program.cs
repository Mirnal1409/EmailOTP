

class Program
{
    static void Main(string[] args)
    {
        var emailOtpModule = new EmailOTP();
        emailOtpModule.Start();

        string userEmail = "user@dso.org.sg";
        int status = emailOtpModule.GenerateOtpEmail(userEmail);

        if (status == StatusConstants.StatusEmailOk)
        {
            Console.WriteLine("OTP sent successfully!");

            var inputStream = new MockInputStream();
            int otpStatus = emailOtpModule.CheckOtp(inputStream,userEmail);

            switch (otpStatus)
            {
                case StatusConstants.StatusOtpOk:
                    Console.WriteLine("OTP validated successfully!");
                    break;
                case StatusConstants.StatusOtpFail:
                    Console.WriteLine("Failed to validate OTP.");
                    break;
                case StatusConstants.StatusOtpTimeout:
                    Console.WriteLine("OTP validation timed out.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("Failed to send OTP.");
        }

        emailOtpModule.Close();
    }
}
