using Hungabor01Website.Utilities.Interfaces;
using Hungabor01Website.Utilities.Classes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace Hungabor01Website.StartupConfiguration
{
  /// <summary>
  /// Configuration for the common components
  /// </summary>
  public class UtilitiesConfiguration : StartupConfigurationBase
  {
    public UtilitiesConfiguration(
      IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
      : base(services, configuration, environment)
    {
    }

    public override void Configure()
    {
      //EmailValidator
      Services.AddTransient<IEmailValidator, EmailValidator>();

      //MessageSenders
      Services.AddTransient<IMessageSender, SmtpEmailSender>(s => new SmtpEmailSender(
        Configuration.GetValue<string>("EmailSender:host"),
        Configuration.GetValue<int>("EmailSender:port"),
        Configuration.GetValue<string>("EmailSender:username"),
        Configuration.GetValue<string>("EmailSender:password"),
        s.GetService<IEmailValidator>(),
        s.GetService<ILogger<SmtpEmailSender>>()
      ));

      //Helpers
      Services.AddTransient<ILoginRegistrationAccountHelper, LoginRegistrationAccountHelper>();
    }
  }
}
