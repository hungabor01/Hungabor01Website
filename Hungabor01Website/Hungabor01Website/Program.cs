using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.AzureAppServices;

namespace Hungabor01Website
{
  /// <summary>
  /// Entry point of the application 
  /// </summary>
  public class Program
  {
    /// <summary>
    /// Main method of the application
    /// </summary>
    /// <param name="args">Command line arguments</param>
    public static void Main(string[] args)
    {
      CreateHostBuilder(args).Build().Run();
    }

    /// <summary>
    /// Configures and makes the application to a web werver
    /// </summary>
    /// <param name="args">Command line arguments passed from the Main method</param>
    /// <returns></returns>
    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .ConfigureLogging(logging =>
        {
          logging.AddAzureWebAppDiagnostics();
        })
        .ConfigureServices(serviceCollection =>
        {
          serviceCollection.Configure<AzureBlobLoggerOptions>(options =>
          {
            options.BlobName = "log.txt";
          });
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
          webBuilder.UseStartup<Startup>();
        });
  }
}
