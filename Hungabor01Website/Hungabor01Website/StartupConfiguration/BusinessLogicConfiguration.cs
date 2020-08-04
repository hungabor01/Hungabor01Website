using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Services.Classes;
using BusinessLogic.ControllerManagers.Interfaces;
using BusinessLogic.ControllerManagers.Classes;

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
            Services.AddTransient<IAccountControllersManager, AccountControllersManager>();

            Services.AddTransient<IEmailValidator, EmailValidator>();

            Services.AddTransient<IEmailSender, SmtpEmailSender>(s => new SmtpEmailSender(
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
