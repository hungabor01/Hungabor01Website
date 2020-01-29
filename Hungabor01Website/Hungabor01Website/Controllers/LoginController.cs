using Hungabor01Website.Utilities;
using Hungabor01Website.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Hungabor01Website.Constants;
using Microsoft.Extensions.Logging;
using Hungabor01Website.Database.Entities;

namespace Hungabor01Website.Controllers
{
  public class LoginController : Controller
  {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger logger;
    private readonly ILoginRegistrationAccountHelper helper;

    public LoginController(
      UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager,
      IServiceProvider serviceProvider,
      ILogger<LoginController> logger,
      ILoginRegistrationAccountHelper helper)
    {
      this.userManager = userManager;
      this.signInManager = signInManager;
      this.serviceProvider = serviceProvider;
      this.logger = logger;
      this.helper = helper;
    }

    /// <summary>
    /// Login get method
    /// </summary>
    /// <param name="returnUrl">The redirection url after authentication</param>
    /// <returns>Login view</returns>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string returnUrl)
    {
      LoginViewModel model = new LoginViewModel
      {
        ReturnUrl = returnUrl
      };

      return View(model);
    }

    /// <summary>
    /// Login post method
    /// </summary>
    /// <param name="model">ViewModel of the login</param>
    /// <returns>Asynchronous login brings to the previously requested url</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      var user = await userManager.FindByEmailAsync(model.UsernameOrEmail);
      if (user == null)
      {
        user = await userManager.FindByNameAsync(model.UsernameOrEmail);
      }

      if (user == null)
      {
        ModelState.AddModelError(string.Empty, Strings.InvalidLogin);
        logger.LogWarning(EventIds.LoginError, Strings.LoginUsernameError);
        return View(model);
      }

      if (!user.EmailConfirmed && (await userManager.CheckPasswordAsync(user, model.Password)))
      {
        ModelState.AddModelError(string.Empty, Strings.EmailNotConfirmed);
        return View(model);
      }

      var result = await signInManager.PasswordSignInAsync(
        user.UserName, model.Password, model.RememberMe, false);

      if (result.Succeeded)
      {
        helper.LogUserActionToDatabase(user, UserActionType.Login, Strings.LoginBuiltIn);
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
          returnUrl = Url.Content("~/");
        }
        return LocalRedirect(returnUrl);
      }
      else
      {
        ModelState.AddModelError(string.Empty, Strings.InvalidLogin);
        logger.LogWarning(EventIds.LoginError, Strings.LoginPasswordError);
        return View(model);
      }
    }

    /// <summary>
    /// Action, which is executed when clicking on an external provider
    /// </summary>
    /// <param name="provider">The name of the external login provider</param>
    /// <param name="returnUrl">The redirection url after authentication</param>
    /// <returns>Redirects to the provider's login page</returns>
    [AllowAnonymous]
    public IActionResult ExternalLogin(string provider, string returnUrl, string source)
    {
      var redirectUrl = Url.Action(
        "ExternalLoginCallback", "Login", new { ReturnUrl = returnUrl, Source = source });

      var properties =
        signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

      return new ChallengeResult(provider, properties);
    }

    /// <summary>
    /// The callback action after the external authentication
    /// </summary>
    /// <param name="returnUrl">The redirection url after authentication</param>
    /// <param name="remoteError">Any error coming from the external provider</param>
    /// <returns></returns>
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback(
      string returnUrl = null, string source = null, string remoteError = null)
    {
      returnUrl ??= Url.Content("~/");

      var info = await CheckExternalErrorsAsny(remoteError);
      
      if (info == null)
      {
        if (source == "Login")
        {
          return View("Login", new LoginViewModel { ReturnUrl = returnUrl });
        }
        else
        {
          return RedirectToAction("Register" , "Register", new RegisterViewModel());
        }
      }

      var signInResult = await signInManager.ExternalLoginSignInAsync(
        info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

      if (signInResult.Succeeded)
      {
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var user = await userManager.FindByEmailAsync(email);

        helper.LogUserActionToDatabase(
          user, UserActionType.Login, string.Format(Strings.LoginExternal, info.LoginProvider));

        return LocalRedirect(returnUrl);
      }
      else
      {
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);

        if (email == null)
        {
          ViewBag.ErrorMessage = string.Format(Strings.EmailClaimError, info.LoginProvider);
          logger.LogWarning(EventIds.ExternalLoginCallbackEmailError,
            string.Format(Strings.ExternalLoginCallbackEmailError, info.LoginProvider));
          return View("Error");
        }
        else
        {
          return await RegisterAndLoginAsync(returnUrl, info, email);
        }
      }
    }

    private async Task<ExternalLoginInfo> CheckExternalErrorsAsny(string remoteError)
    {
      string error = null;
      ExternalLoginInfo info = null;

      if (remoteError != null)
      {
        error = string.Format(Strings.ExternalProviderError, remoteError);
      }
      else
      {
        info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
          error = Strings.ExternalLoginError;
        }
      }

      if (!string.IsNullOrWhiteSpace(error))
      {
        ModelState.AddModelError(string.Empty, error);
        logger.LogWarning(EventIds.ExternalLoginCallbackError, error);
      }

      return info;
    }

    private async Task<IActionResult> RegisterAndLoginAsync(string returnUrl, ExternalLoginInfo info, string email)
    {
      var user = await userManager.FindByEmailAsync(email);

      if (user == null)
      {
        user = new ApplicationUser
        {
          UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
          Email = info.Principal.FindFirstValue(ClaimTypes.Email)
        };

        var result = await userManager.CreateAsync(user);

        if (result.Succeeded)
        {
          helper.LogUserActionToDatabase(user, UserActionType.Registration);
          var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
          await userManager.ConfirmEmailAsync(user, token);
        }
        else
        {
          var model = new RegisterViewModel();
          foreach (var error in result.Errors)
          {
            ModelState.AddModelError(string.Empty, error.Description);
            logger.LogWarning(EventIds.RegisterExternalCreateUserError,
              string.Format(Strings.AccountError, error.Description, user.UserName));
          }

          return RedirectToAction("Register", "Register", model);
        }
      }

      await userManager.AddLoginAsync(user, info);
      await signInManager.SignInAsync(user, isPersistent: false);

      helper.LogUserActionToDatabase(
          user, UserActionType.Login, string.Format(Strings.LoginExternal, info.LoginProvider));

      return LocalRedirect(returnUrl);
    }
  }
}