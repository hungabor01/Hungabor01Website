using Database.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Hungabor01Website.Tests.Helpers
{
    public class DatabaseHelper : IDisposable
    {
        public AppDbContext Context { get; }

        public DatabaseHelper(IConfiguration configuration)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>();
            options.UseSqlServer(configuration.GetConnectionString("AppDbContextLocal"));
            Context = new AppDbContext(options.Options);
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}