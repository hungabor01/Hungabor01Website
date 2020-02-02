using Hungabor01Website.Constants;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Hungabor01Website.Utilities
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

    private readonly ILogger logger;

    public SmtpEmailSender(
      string host, int port,
      string username, string password,
      ILogger<SmtpEmailSender> logger)
    {
      this.host = host;
      this.port = port;
      this.username = username;
      this.password = password;
      this.logger = logger;
    }

    public bool SendMessage(string emailAddress, string subject, string message)
    {
      if (IsValidEmail(emailAddress))
      {
        var mimeMessage = CreateMimeMessage(emailAddress, subject, message);
        return SendMimeMessage(mimeMessage);
      }
      else
      {
        return false;
      }
    }

    private bool IsValidEmail(string email)
    {
      if (string.IsNullOrWhiteSpace(email))
        return false;

      try
      {
        email = Regex.Replace(email, @"(@)(.+)$",
          DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

        return Regex.IsMatch(email,
            Strings.EmailRegexPattern,
            RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
      }
      catch (RegexMatchTimeoutException e)
      {
        return false;
      }
      catch (ArgumentException e)
      {
        return false;
      }
    }

    private string DomainMapper(Match match)
    {
      var idn = new IdnMapping();

      var domainName = idn.GetAscii(match.Groups[2].Value);

      return match.Groups[1].Value + domainName;
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
