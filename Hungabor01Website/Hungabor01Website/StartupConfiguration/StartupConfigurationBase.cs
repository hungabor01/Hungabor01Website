using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hungabor01Website.StartupConfiguration
{
    public abstract class StartupConfigurationBase
    {
        protected IServiceCollection Services { get; }
        protected IConfiguration Configuration { get; }
        protected IWebHostEnvironment Environment { get; }

        public StartupConfigurationBase(
            IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            Services = services;
            Configuration = configuration;
            Environment = environment;
        }

        public abstract void Configure();
    }
}
