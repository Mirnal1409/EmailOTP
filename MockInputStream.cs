
public class MockInputStream : IInputStream
{
    public string ReadOtp(CancellationToken cancellationToken)
    {
        try
        {
            Console.Write("Enter OTP: ");
            using (var reader = new StreamReader(Console.OpenStandardInput()))
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (Console.KeyAvailable)
                    {
                        return reader.ReadLine()?.Trim() ?? string.Empty;
                    }
                    Task.Delay(100, cancellationToken).Wait(); // Periodic check
                }

                throw new OperationCanceledException("Timeout waiting for OTP.");
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
    }
}
