using Hungabor01Website.Database;
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
      Context = CreateContext(configuration);      
    }

    private AppDbContext CreateContext(IConfiguration configuration)
    {
      var options = new DbContextOptionsBuilder<AppDbContext>();
      options.UseSqlServer(configuration.GetConnectionString("WebsiteDbContextLocal"));
      return new AppDbContext(options.Options);
    }

    public void Dispose()
    {
      Context.Dispose();
    }
  }
}