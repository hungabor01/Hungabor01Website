using Hungabor01Website.Constants;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Reflection;

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

    public bool SendMessage(string email, string subject, string message)
    {
      var mimeMessage = CreateMimeMessage(email, subject, message);
      return SendMimeMessage(mimeMessage);
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

    private bool SendMimeMessage(MimeMessage mimeMessage)
    {
      mimeMessage.ThrowExceptionIfNull(nameof(mimeMessage));

      using (var client = new SmtpClient())
      {
        try
        {
          client.Connect(host, port);
          client.Authenticate(username, password);
          client.Send(mimeMessage);          
          logger.LogInformation(EventIds.ConfirmationEmailSent, mimeMessage.ToString());
          return true;
        }
        catch (Exception ex)
        {
          logger.LogError(EventIds.ConfirmationEmailSentError, ex, mimeMessage.ToString());
          return false;
        }
        finally
        {
          if (client.IsConnected)
            client.Disconnect(true);
        }
      }
    }
  }
}
