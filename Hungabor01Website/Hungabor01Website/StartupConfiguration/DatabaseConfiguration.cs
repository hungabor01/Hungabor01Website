using Hungabor01Website.Database;
using Hungabor01Website.Database.Repositories.Interfaces;
using Hungabor01Website.Database.Repositories.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Hungabor01Website.StartupConfiguration
{
  /// <summary>
  /// Configuration for the database specific components
  /// </summary>
  public class DatabaseConfiguration : StartupConfigurationBase
  {
    public DatabaseConfiguration(
      IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
      : base(services, configuration, environment)
    {
    }

    public override void Configure()
    {
      //Database
      //UnitOfWork
      Services.AddTransient<IUnitOfWork, UnitOfWork>();

      //Repositories
      Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
      Services.AddTransient<IAttachmentRepository, AttachmentRepository>();
      Services.AddTransient<IAccountHistoryRepository, AccountHistoryRepository>();

      //DbContext
      if (Environment.IsDevelopment())
        Services.AddDbContext<AppDbContext>(options =>
          options.UseSqlServer(Configuration.GetConnectionString("WebsiteDbContextLocal")),
          ServiceLifetime.Transient);
      else
        Services.AddDbContext<AppDbContext>(options =>
          options.UseSqlServer(Configuration.GetConnectionString("WebsiteDbContextAzure")),
          ServiceLifetime.Transient);
    }
  }
}
