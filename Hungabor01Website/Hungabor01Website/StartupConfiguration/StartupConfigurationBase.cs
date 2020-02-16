using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hungabor01Website.StartupConfiguration
{
  /// <summary>
  /// Base class for the different types of configurations in the DI registration
  /// </summary>
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

    /// <summary>
    /// Configures and registers the components
    /// </summary>
    public abstract void Configure();
  }
}
