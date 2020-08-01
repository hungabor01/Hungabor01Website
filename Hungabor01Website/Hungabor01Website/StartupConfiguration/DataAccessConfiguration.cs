using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using DataAccess.Managers.Classes;
using DataAccess.Managers.Interfaces;

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
            Services.AddTransient<IAccountManager, AccountManager>();
        }
    }
}
