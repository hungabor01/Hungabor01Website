using Hungabor01Website.Database.Repositories.Interfaces;
using Hungabor01Website.Database.Repositories.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Hungabor01Website.Database.UnitOfWork;
using Hungabor01Website.Database.Core;

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
                    options.UseSqlServer(Configuration.GetConnectionString("AppDbContextLocal")),
                    ServiceLifetime.Transient);
            }
            else
            {
                Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("AppDbContextAzure")),
                    ServiceLifetime.Transient);
            }
        }
    }
}
