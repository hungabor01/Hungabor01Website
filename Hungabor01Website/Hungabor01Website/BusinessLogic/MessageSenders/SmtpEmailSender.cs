using Hungabor01Website.BusinessLogic.MessageSenders.EmailValidator;
using Hungabor01Website.BusinessLogic;
using Hungabor01Website.Constants;
using Hungabor01Website.Constants.Strings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Hungabor01Website.BusinessLogic.MessageSenders
{
    public class SmtpEmailSender : IMessageSender
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;

        private readonly IEmailValidator _emailValidator;
        private readonly ILogger _logger;

        public SmtpEmailSender(
            string host, int port,
            string username, string password,
            IEmailValidator emailValidator,
            ILogger<SmtpEmailSender> logger)
        {
            _host = host;
            _port = port;
            _username = username;
            _password = password;
            _emailValidator = emailValidator;
            _logger = logger;
        }

        public async Task<bool> SendMessageAsync(string emailAddress, string subject, string message)
        {
            if (!_emailValidator.IsValidEmail(emailAddress))
            {
                _logger.LogWarning(EventIds.InvalidEmail, string.Format(Strings.InvalidEmail, emailAddress));
                return false;
            }
            
            var mimeMessage = CreateMimeMessage(emailAddress, subject, message);
            return await SendMimeMessageAsync(mimeMessage);
        }    

        private MimeMessage CreateMimeMessage(string emailAddress, string subject, string message)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(Assembly.GetEntryAssembly().GetName().Name, _username));
            mimeMessage.To.Add(new MailboxAddress(emailAddress, emailAddress));
            mimeMessage.Subject = subject;
            mimeMessage.Body = new TextPart("plain") { Text = message };
            return mimeMessage;
        }

        private async Task<bool> SendMimeMessageAsync(MimeMessage mimeMessage)
        {
            mimeMessage.ThrowExceptionIfNull(nameof(mimeMessage));

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_host, _port);
                    await client.AuthenticateAsync(_username, _password);
                    await client.SendAsync(mimeMessage);          
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(EventIds.SendEmailError, ex, mimeMessage.ToString());
                    return false;
                }
                finally
                {
                    if (client.IsConnected)
                    {
                        await client.DisconnectAsync(true);
                    } 
                }
            }
        }
    }
}
