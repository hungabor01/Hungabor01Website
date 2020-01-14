using Hungabor01Website.Database;
using Hungabor01Website.Database.Entities;
using Hungabor01Website.Database.Repositories;
using Hungabor01Website.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Hungabor01Website
{
  /// <summary>
  /// Initialization of the web server
  /// </summary>
  public class Startup
  {
    private readonly IConfiguration configuration;

    public Startup(IConfiguration configuration)
    {
      this.configuration = configuration;
    }    

    /// <summary>
    /// Setup the dependency injection here
    /// </summary>
    /// <param name="services">The dependency injection container</param>
    public void ConfigureServices(IServiceCollection services)
    {
      //MVC
      services.AddControllersWithViews(config =>
      {
        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        config.Filters.Add(new AuthorizeFilter(policy));
      });

      //Https
      services.AddHsts(options =>
      {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(365);
      });

      //Database
      //UnitOfWork
      services.AddScoped<IUnitOfWork, UnitOfWork>();

      //Repositories
      services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
      services.AddScoped<ITestEntityRepository, TestEntityRepository>();

      //DbContext
      if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        services.AddDbContext<WebsiteDbContext>(options =>
          options.UseSqlServer(configuration.GetConnectionString("WebsiteDbContextLocal")));
      else
        services.AddDbContext<WebsiteDbContext>(options =>
          options.UseSqlServer(configuration.GetConnectionString("WebsiteDbContextAzure")));

      //Entities
      services.AddScoped<TestEntity>();

      //Adds the user and role object to the db context
      services.AddIdentity<IdentityUser, IdentityRole>(options =>
      {
        options.SignIn.RequireConfirmedEmail = true;
      })
      .AddEntityFrameworkStores<WebsiteDbContext>()
      .AddDefaultTokenProviders();

      //Password complexity
      services.Configure<IdentityOptions>(options =>
      {
        options.Password.RequiredLength = 10;
        options.Password.RequiredUniqueChars = 3;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = true;
      });

      //Add external login providers
      services.AddAuthentication()
      .AddGoogle(options =>
      {
        options.ClientId = configuration.GetValue<string>("ExternalLoginProviders:Google:ClientId");
        options.ClientSecret = configuration.GetValue<string>("ExternalLoginProviders:Google:ClientSecret");
      })
      .AddFacebook(options =>
      {
        options.AppId = configuration.GetValue<string>("ExternalLoginProviders:Facebook:AppId");
        options.AppSecret = configuration.GetValue<string>("ExternalLoginProviders:Facebook:AppSecret");
      });

      //MessageSenders
      services.AddScoped<IMessageSender, EmailSender>(s => new EmailSender(
        configuration.GetValue<string>("EmailSender:host"),
        configuration.GetValue<int>("EmailSender:port"),
        configuration.GetValue<string>("EmailSender:username"),
        configuration.GetValue<string>("EmailSender:password"),
        s.GetService<ILogger<EmailSender>>()
      ));
    }

    /// <summary>
    /// Setup of the request processing pipeline
    /// </summary>
    /// <param name="app">Service of the application</param>
    /// <param name="env">Service of the environment</param>
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

      app.UseAuthentication();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
