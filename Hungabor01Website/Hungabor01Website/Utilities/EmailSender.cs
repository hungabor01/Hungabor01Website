using Hungabor01Website.Constants;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Hungabor01Website.Utilities
{
  /// <summary>
  /// Sends message as email
  /// </summary>
  public class EmailSender : IMessageSender
  {
    private readonly string host;
    private readonly int port;
    private readonly string username;
    private readonly string password;

    private readonly ILogger logger;

    public EmailSender(
      string host, int port,
      string username, string password,
      ILogger<EmailSender> logger)
    {
      this.host = host;
      this.port = port;
      this.username = username;
      this.password = password;
      this.logger = logger;
    }

    public async Task<bool> SendMessageAsync(string email, string subject, string message)
    {
      var mimeMessage = CreateMimeMessage(email, subject, message);
      return await SendMimeMessageAsync(mimeMessage);
    }

    private MimeMessage CreateMimeMessage(string email, string subject, string message)
    {
      var mimeMessage = new MimeMessage();

      mimeMessage.From.Add(
        new MailboxAddress(Assembly.GetEntryAssembly().GetName().Name, username));

      mimeMessage.To.Add(
        new MailboxAddress(email, email));

      mimeMessage.Subject = subject;

      mimeMessage.Body = new TextPart("plain")
      {
        Text = message
      };

      return mimeMessage;
    }

    private async Task<bool> SendMimeMessageAsync(MimeMessage mimeMessage)
    {
      using (var client = new SmtpClient())
      {
        try
        {
          client.Connect(host, port);
          client.Authenticate(username, password);
          await client.SendAsync(mimeMessage);
          await client.DisconnectAsync(true);
          logger.LogInformation(EventIds.ConfirmationEmailSent, mimeMessage.ToString());
          return true;
        }
        catch (Exception ex)
        {
          logger.LogError(EventIds.ConfirmationEmailSentError, ex, mimeMessage.ToString());
          return false;
        }
      }
    }
  }
}
