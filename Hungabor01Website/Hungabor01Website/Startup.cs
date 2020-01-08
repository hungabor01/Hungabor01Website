using Hungabor01Website.Database;
using Hungabor01Website.Database.Entities;
using Hungabor01Website.Database.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Hungabor01Website
{
  public class Startup
  {
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }    

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllersWithViews();

      services.AddHsts(options =>
      {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(365);
      });

      //UnitOfWork
      services.AddScoped<IUnitOfWork, UnitOfWork>();

      //Repositories
      services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
      services.AddScoped<ITestEntityRepository, TestEntityRepository>();

      //DbContext
      if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        services.AddDbContext<WebsiteDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("WebsiteDbContextAzure")));
      else
        services.AddDbContext<WebsiteDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("WebsiteDbContextLoacl")));

      //Entities
      services.AddScoped<TestEntity>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Error/UnhandledError");
        app.UseStatusCodePagesWithReExecute("/Error/ErrorCodeHandler/{0}");        

        app.UseHsts();
      }

      app.UseHttpsRedirection();

      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
