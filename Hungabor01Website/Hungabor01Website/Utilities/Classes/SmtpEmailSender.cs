using Hungabor01Website.Constants;
using Hungabor01Website.Utilities.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Reflection;

namespace Hungabor01Website.Utilities.Classes
{
  /// <summary>
  /// Sends message as email via smtp
  /// </summary>
  public class SmtpEmailSender : IMessageSender
  {
    private readonly string host;
    private readonly int port;
    private readonly string username;
    private readonly string password;

    private readonly IEmailValidator emailValidator;
    private readonly ILogger logger;

    public SmtpEmailSender(
      string host, int port,
      string username, string password,
      IEmailValidator emailValidator,
      ILogger<SmtpEmailSender> logger)
    {
      this.host = host;
      this.port = port;
      this.username = username;
      this.password = password;
      this.emailValidator = emailValidator;
      this.logger = logger;
    }

    public bool SendMessage(string emailAddress, string subject, string message)
    {
      if (emailValidator.IsValidEmail(emailAddress))
      {
        var mimeMessage = CreateMimeMessage(emailAddress, subject, message);
        return SendMimeMessage(mimeMessage);
      }
      else
      {
        return false;
      }
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
