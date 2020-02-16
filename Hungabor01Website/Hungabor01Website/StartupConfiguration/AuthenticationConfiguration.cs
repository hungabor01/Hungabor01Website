using Hungabor01Website.Database;
using Hungabor01Website.Database.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hungabor01Website.StartupConfiguration
{
  /// <summary>
  /// Configuration for the authentication specific components
  /// </summary>
  public class AuthenticationConfiguration : StartupConfigurationBase
  {
    public AuthenticationConfiguration(
      IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
      : base(services, configuration, environment)
    {
    }

    public override void Configure()
    {
      //Adds the user and role object to the db context
      Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
      {
        options.SignIn.RequireConfirmedEmail = true;
      })
      .AddEntityFrameworkStores<AppDbContext>()
      .AddDefaultTokenProviders();

      //Password complexity
      Services.Configure<IdentityOptions>(options =>
      {
        options.Password.RequiredLength = 10;
        options.Password.RequiredUniqueChars = 3;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = true;
      });

      //Add external login providers
      Services.AddAuthentication()
      .AddGoogle(options =>
      {
        options.ClientId = Configuration.GetValue<string>("ExternalLoginProviders:Google:ClientId");
        options.ClientSecret = Configuration.GetValue<string>("ExternalLoginProviders:Google:ClientSecret");
      })
      .AddFacebook(options =>
      {
        options.AppId = Configuration.GetValue<string>("ExternalLoginProviders:Facebook:AppId");
        options.AppSecret = Configuration.GetValue<string>("ExternalLoginProviders:Facebook:AppSecret");
      });
    }    
  }
}
