using BusinessLogic.Services.Classes;
using BusinessLogic.Services.Interfaces;
using Hungabor01Website.Tests.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Hungabor01Website.Tests.BusinessLogic.Services
{
    public class SmtpEmailSenderTests
    {
        private readonly ConfigurationHelper _configurationHelper;
        private readonly Mock<ILogger<SmtpEmailSender>> _mockLogger;        
        private readonly string _email;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;

        public SmtpEmailSenderTests() 
        {
            _configurationHelper = new ConfigurationHelper();
            _mockLogger = new Mock<ILogger<SmtpEmailSender>>();
            _email = _configurationHelper.Configuration.GetValue<string>("EmailSender:host");
            _port = _configurationHelper.Configuration.GetValue<int>("EmailSender:port");
            _username = _configurationHelper.Configuration.GetValue<string>("EmailSender:username");
            _password = _configurationHelper.Configuration.GetValue<string>("EmailSender:password");
        }

        [Theory]
        [InlineData("hungabor01@gmail.com")]
        public async Task SendMessageAsync_ValidParameters_EmailSent(string emailAddress)
        {
            var sender = CreateSender(true);

            var result = await sender.SendEmailAsync(emailAddress, "Test message", "Test message.");

            Assert.True(result);

            _mockLogger.Verify(l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Never
            );
        }

        [Theory]
        [InlineData("invalid@email@gmail.com")]
        public async Task SendMessageAsync_InvalidParameters_EmailNotSent(string emailAddress)
        {
            var sender = CreateSender(false);

            var result = await sender.SendEmailAsync(emailAddress, "Test message", "Test message.");

            Assert.False(result);

            _mockLogger.Verify(l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once
            );
        }

        [Theory]
        [InlineData("hungabor01@gmail.com")]
        public async Task SendMessageAsync_InvalidSenderCredentials_EmailNotSent(string emailAddress)
        {
            var sender = CreateSender(true, "wrongPassword");

            var result = await sender.SendEmailAsync(emailAddress, "Test message", "Test message.");

            Assert.False(result);

            _mockLogger.Verify(l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once
            );
        }

        private SmtpEmailSender CreateSender(bool isValidEmail, string password = null)
        {    
            if (string.IsNullOrWhiteSpace(password))
            {
                password = _password;
            }

            var mockValidator = new Mock<IEmailValidator>();
            mockValidator.Setup(ev => ev.IsValidEmail(It.IsAny<string>())).Returns(isValidEmail);  

            var sender = new SmtpEmailSender(
                _email,
                _port,
                _username,
                password,
                mockValidator.Object,
                _mockLogger.Object
            );

            return sender;
        }
    }
}
