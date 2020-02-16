using Hungabor01Website.Utilities.Classes;
using Xunit;

namespace Hungabor01Website.Tests.Utilities
{
  public class EmailValidatorTests
  {
    private readonly EmailValidator validator;

    public EmailValidatorTests()
    {
      validator = new EmailValidator();
    }

    [Theory]
    [InlineData("hungabor01@gmail.com")]
    public void IsValidEmail_ValidEmail_ReturnTrue(string emailAddress)
    {   
      var result = validator.IsValidEmail(emailAddress);

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
      var result = validator.IsValidEmail(emailAddress);

      Assert.False(result);
    }
  }
}
