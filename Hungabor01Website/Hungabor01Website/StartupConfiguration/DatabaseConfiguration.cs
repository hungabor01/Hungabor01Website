using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Database.UnitOfWork;
using Database.Repositories.Interfaces;
using Database.Repositories.Classes;
using Database.Core;

namespace Hungabor01Website.StartupConfiguration
{
    public class DatabaseConfiguration : StartupConfigurationBase
    {
        public DatabaseConfiguration(
            IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment)
            : base(services, configuration, environment)
        {
        }

        public override void Configure()
        {
            Services.AddTransient<IUnitOfWork, UnitOfWork>();

            Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            Services.AddTransient<IAttachmentRepository, AttachmentRepository>();
            Services.AddTransient<IAccountHistoryRepository, AccountHistoryRepository>();

            if (Environment.IsDevelopment())
            {
                Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("AppDbContextLocal"), b => b.MigrationsAssembly("Hungabor01Website")),
                    ServiceLifetime.Transient);
            }
            else
            {
                Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("AppDbContextAzure"), b => b.MigrationsAssembly("Hungabor01Website")),
                    ServiceLifetime.Transient);
            }
        }
    }
}
