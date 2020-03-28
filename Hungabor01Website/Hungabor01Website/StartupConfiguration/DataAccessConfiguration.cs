using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Hungabor01Website.DataAccess.Managers.Classes;
using Hungabor01Website.DataAccess.Managers.Interfaces;

namespace Hungabor01Website.StartupConfiguration
{
    public class DataAccessConfiguration : StartupConfigurationBase
    {
        public DataAccessConfiguration(
            IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment)
            : base(services, configuration, environment)
        {
        }

        public override void Configure()
        {
            Services.AddTransient<IRegistrationManager, RegistrationManager>();
            Services.AddTransient<ILoginManager, LoginManager>();
            Services.AddTransient<IAccountManager, AccountManager>();
        }
    }
}
