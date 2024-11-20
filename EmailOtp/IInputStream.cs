namespace EmailOTPModule{
public interface IInputStream
{
    string ReadOtp(CancellationToken cancellationToken);
}

}
