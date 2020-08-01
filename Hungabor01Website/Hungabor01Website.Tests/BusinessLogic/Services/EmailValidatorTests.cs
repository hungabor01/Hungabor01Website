using BusinessLogic.Services.Classes;
using Xunit;

namespace Hungabor01Website.Tests.BusinessLogic.Services
{
    public class EmailValidatorTests
    {
        private readonly EmailValidator _validator;

        public EmailValidatorTests()
        {
            _validator = new EmailValidator();
        }

        [Theory]
        [InlineData("hungabor01@gmail.com")]
        public void IsValidEmail_ValidEmail_ReturnTrue(string emailAddress)
        {   
            var result = _validator.IsValidEmail(emailAddress);
            Assert.True(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("js@internal@proseware.com")]
        [InlineData("j..s@proseware.com")]
        [InlineData("js*@proseware.com")]
        [InlineData("js@proseware..com")]
        [InlineData("j.@server1.proseware.com")]
        public void IsValidEmail_InvalidEmail_ReturnFalse(string emailAddress)
        {
            var result = _validator.IsValidEmail(emailAddress);
            Assert.False(result);
        }
    }
}
