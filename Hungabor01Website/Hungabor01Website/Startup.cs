using Hungabor01Website.StartupConfiguration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Hungabor01Website
{
  /// <summary>
  /// Initialization of the web server
  /// </summary>
  public class Startup
  {
    private readonly IConfiguration configuration;
    private readonly IWebHostEnvironment environment;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
      this.configuration = configuration;
      this.environment = environment;
    }    

    /// <summary>
    /// Setup of the dependency injection
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

      //Security
      services.AddHsts(options =>
      {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(365);
      });

      services.AddHttpsRedirection(options =>
      {
        options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
        options.HttpsPort = 5001;
      });

      //Database
      var databaseConfig = new DatabaseConfiguration(services, configuration, environment);
      databaseConfig.Configure();

      //Authentication
      var authenticationConfig = new AuthenticationConfiguration(services, configuration, environment);
      authenticationConfig.Configure();

      //Utilities
      var utilitiesConfig = new UtilitiesConfiguration(services, configuration, environment);
      utilitiesConfig.Configure();
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
