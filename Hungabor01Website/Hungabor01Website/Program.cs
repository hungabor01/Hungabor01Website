using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.AzureAppServices;

namespace Hungabor01Website
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .ConfigureLogging(logging =>
        { 
          logging.AddAzureWebAppDiagnostics();

          //These are overridden by the Azure, so actually don't do aníthing in production
          logging.SetMinimumLevel(LogLevel.Information);
          logging.AddFilter("System", LogLevel.Warning);
          logging.AddFilter("Microsoft", LogLevel.Warning);
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
