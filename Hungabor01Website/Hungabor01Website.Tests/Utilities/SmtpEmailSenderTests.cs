using Hungabor01Website.Tests.Helpers;
using Hungabor01Website.Utilities.Classes;
using Hungabor01Website.Utilities.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Hungabor01Website.Tests.Utilities
{
  public class SmtpEmailSenderTests
  {
    private readonly ConfigurationHelper configurationHelper;

    private readonly string email;
    private readonly int port;
    private readonly string username;
    private readonly string password;

    public SmtpEmailSenderTests() 
    {
      configurationHelper = new ConfigurationHelper();

      email = configurationHelper.Configuration.GetValue<string>("EmailSender:host");
      port = configurationHelper.Configuration.GetValue<int>("EmailSender:port");
      username = configurationHelper.Configuration.GetValue<string>("EmailSender:username");
      password = configurationHelper.Configuration.GetValue<string>("EmailSender:password");
    }

    [Theory]
    [InlineData("hungabor01surelynorvalid@gmail.com")]
    public void SendMessage_ValidParameters_EmailSent(string emailAddress)
    {
      var sender = CreateSender(true);

      var result = sender.SendMessage(
        emailAddress,
        "Test message",
        "Test message."
      );

      Assert.True(result);
    }

    [Theory]
    [InlineData("invalid@email@gmail.com")]
    public void SendMessage_InvalidParameters_EmailNotSent(string emailAddress)
    {
      var sender = CreateSender(false);

      var result = sender.SendMessage(
        emailAddress,
        "Test message",
        "Test message."
      );

      Assert.False(result);
    }

    private SmtpEmailSender CreateSender(bool isValidEmail)
    {            
      var mockValidator = new Mock<IEmailValidator>();
      mockValidator.Setup(x => x.IsValidEmail(It.IsAny<string>())).Returns(isValidEmail);
      
      var mockLogger = new Mock<ILogger<SmtpEmailSender>>();

      var sender = new SmtpEmailSender(
        email,
        port,
        username,
        password,
        mockValidator.Object,
        mockLogger.Object
        );

      return sender;
    }
  }
}
