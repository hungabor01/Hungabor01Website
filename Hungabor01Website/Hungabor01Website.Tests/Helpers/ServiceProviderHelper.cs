using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Hungabor01Website.Tests.Helpers
{
    public class ServiceProviderHelper
    {
        public IServiceProvider ServiceProvider { get; }

        public ServiceProviderHelper(IConfiguration configuration)
        {
            var contentPath = Directory.GetCurrentDirectory();
            contentPath = contentPath.Substring(0, contentPath.IndexOf("bin") - 7);

            var builder = new WebHostBuilder()
                .UseContentRoot(contentPath)
                .UseEnvironment("Development")
                .UseConfiguration(configuration)
                .UseStartup<Startup>();

            ServiceProvider = builder.Build().Services;
        }
    }
}
