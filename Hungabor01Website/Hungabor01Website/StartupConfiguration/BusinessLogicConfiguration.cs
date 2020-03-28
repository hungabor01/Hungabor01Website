using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Hungabor01Website.BusinessLogic.MessageSenders;
using Hungabor01Website.BusinessLogic.MessageSenders.EmailValidator;

namespace Hungabor01Website.StartupConfiguration
{
    public class BusinessLogicConfiguration : StartupConfigurationBase
    {
        public BusinessLogicConfiguration(
            IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment)
            : base(services, configuration, environment)
        {
        }

        public override void Configure()
        {
            Services.AddTransient<IEmailValidator, EmailValidator>();

            Services.AddTransient<IMessageSender, SmtpEmailSender>(s => new SmtpEmailSender(
                Configuration.GetValue<string>("EmailSender:host"),
                Configuration.GetValue<int>("EmailSender:port"),
                Configuration.GetValue<string>("EmailSender:username"),
                Configuration.GetValue<string>("EmailSender:password"),
                s.GetService<IEmailValidator>(),
                s.GetService<ILogger<SmtpEmailSender>>()
            ));
        }
    }
}
