﻿using Microsoft.Extensions.Configuration;
using System.IO;

namespace Hungabor01Website.Tests.Helpers
{
    public class ConfigurationHelper
    {
        public IConfiguration Configuration { get; }

        public ConfigurationHelper()
        {
            Configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddEnvironmentVariables()
              .Build();
        }
    }
}