using Moq;
using Xunit;
using EmailOTPModule;

namespace EmailOtpModule.Tests
{
    public class EmailOtpModuleTests
    {
        
        [Fact]
        public void GenerateOtpEmail_ValidEmail_ReturnsStatusEmailOk()
        {
            // Arrange
            var emailOtpModule = new EmailOTP();
            var userEmail = "user@dso.org.sg";
            
            // Act
            var result = emailOtpModule.GenerateOtpEmail(userEmail);
            
            // Assert
            Assert.Equal(StatusConstants.StatusEmailOk, result.Item1);
        }

        [Fact]
        public void GenerateOtpEmail_InValidEmail_ReturnsStatusEmailOk()
        {
            // Arrange
            var emailOtpModule = new EmailOTP();
            var userEmail = "user@gmail.com";
            
            // Act
            var result = emailOtpModule.GenerateOtpEmail(userEmail);
            
            // Assert
            Assert.Equal(StatusConstants.StatusEmailInvalid, result.Item1);
        }

        [Fact]
        public void CheckOtp_ValidOtp_ReturnsStatusOtpOk()
        {
            // Arrange
            var emailOtpModule = new EmailOTP();
            var userEmail = "user@dso.org.sg";
            var res=emailOtpModule.GenerateOtpEmail(userEmail); 
            
            var mockInputStream = new Mock<IInputStream>();
            mockInputStream.Setup(m => m.ReadOtp(It.IsAny<CancellationToken>()))
                .Returns(res.Item2);  
            
            // Act
            var result = emailOtpModule.CheckOtp(mockInputStream.Object, userEmail);
            
            // Assert
            Assert.Equal(StatusConstants.StatusOtpOk, result);
        }

        [Fact]
        public void CheckOtp_WrongOtp10Tries_ReturnsStatusOtpFail()
        {
            // Arrange
            var emailOtpModule = new EmailOTP();
            var userEmail = "user@dso.org.sg";
            emailOtpModule.GenerateOtpEmail(userEmail); // Assume OTP generated and stored
            
            // Create mock input stream that simulates incorrect OTPs
            var mockInputStream = new Mock<IInputStream>();
            mockInputStream.SetupSequence(m => m.ReadOtp(It.IsAny<CancellationToken>()))
                .Returns("000000")  // 1st attempt
                .Returns("000000")  // 2nd attempt
                .Returns("000000")  // 3rd attempt
                .Returns("000000")  // 4th attempt
                .Returns("000000")  // 5th attempt
                .Returns("000000")  // 6th attempt
                .Returns("000000")  // 7th attempt
                .Returns("000000")  // 8th attempt
                .Returns("000000")  // 9th attempt
                .Returns("000000"); // 10th attempt
            
            // Act
            var result = emailOtpModule.CheckOtp(mockInputStream.Object, userEmail);
            
            // Assert
            Assert.Equal(StatusConstants.StatusOtpFail, result);
        }


        [Fact]
        public void CheckOtp_NoOTPFoundForEmail_ReturnsStatusOtpFail()
        {
            // Arrange
            var emailOtpModule = new EmailOTP();
            var userEmail = "user@dso.org.sg";
            var res=emailOtpModule.GenerateOtpEmail(userEmail); 
            
            var mockInputStream = new Mock<IInputStream>();
            mockInputStream.Setup(m => m.ReadOtp(It.IsAny<CancellationToken>()))
                .Returns(res.Item2);  
            
            // Act
            var result = emailOtpModule.CheckOtp(mockInputStream.Object, "user1@dso.org.sg");
            
            // Assert
            Assert.Equal(StatusConstants.StatusOtpFail, result);
        }

    }
}

  