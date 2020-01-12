using Hungabor01Website.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hungabor01Website.Controllers
{
  /// <summary>
  /// Manages the account related actions
  /// </summary>
  public class AccountController : Controller
  {
    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManager;

    public AccountController(
      UserManager<IdentityUser> userManager,
      SignInManager<IdentityUser> signInManager)
    {
      this.userManager = userManager;
      this.signInManager = signInManager;
    }

    /// <summary>
    /// Registration get method
    /// </summary>
    /// <returns>Register view</returns>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
      return View();
    }

    /// <summary>
    /// Registration post method
    /// </summary>
    /// <param name="model">ViewModel of the registration</param>
    /// <returns>Asynchronous regisrtation redirects to the home view</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
      if (ModelState.IsValid)
      {
        var user = new IdentityUser
        {
          UserName = model.Username,
          Email = model.Email
        };

        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
          await signInManager.SignInAsync(user, isPersistent: false);
          return RedirectToAction("index", "home");
        }

        foreach (var error in result.Errors)
        {
          ModelState.AddModelError(string.Empty, error.Description);
        }
      }

      return View(model);
    }

    [AcceptVerbs("Get", "Post")]
    [AllowAnonymous]
    public async Task<IActionResult> IsUsernameInUse(string username)
    {
      var user = await userManager.FindByNameAsync(username);

      if (user == null)
      {
        return Json(true);
      }
      else
      {
        return Json($"Username {username} is already in use.");
      }
    }

    [AcceptVerbs("Get", "Post")]
    [AllowAnonymous]
    public async Task<IActionResult> IsEmailInUse(string email)
    {
      var user = await userManager.FindByEmailAsync(email);

      if (user == null)
      {
        return Json(true);
      }
      else
      {
        return Json($"Email {email} is already in use.");
      }
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
    /// Action, which is executed when clicking on an external provider
    /// </summary>
    /// <param name="provider">The name of the external login provider</param>
    /// <param name="returnUrl">The redirection url after authentication</param>
    /// <returns>Redirects to the provider's login page</returns>
    [AllowAnonymous]    
    public IActionResult ExternalLogin(string provider, string returnUrl, string source)
    {
      var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl, Source = source });
      var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
      return new ChallengeResult(provider, properties);
    }

    /// <summary>
    /// The callback action after the external authentication
    /// </summary>
    /// <param name="returnUrl">The redirection url after authentication</param>
    /// <param name="remoteError">Any error coming from the external provider</param>
    /// <returns></returns>
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string source = null, string remoteError = null)
    {
      returnUrl ??= Url.Content("~/");

      string error = null;

      if (remoteError != null)
      {
        error = $"Error from external provider: {remoteError}";
      }

      var info = await signInManager.GetExternalLoginInfoAsync();
      if (info == null && remoteError == null)
      {
        error = "Error loading external login information.";
      }

      if (!string.IsNullOrWhiteSpace(error))
      {
        ModelState.AddModelError(string.Empty, error);
        if (source == "Login")
        {
          return View(source, new LoginViewModel { ReturnUrl = returnUrl });
        }
        else
        {
          return View(source, new RegisterViewModel());
        }
      }

      var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

      if (signInResult.Succeeded)
      {
        return LocalRedirect(returnUrl);
      }
      else
      {
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);

        if (email != null)
        {
          var user = await userManager.FindByEmailAsync(email);

          if (user == null)
          {
            user = new IdentityUser
            {
              UserName = info.Principal.FindFirstValue(ClaimTypes.Name),
              Email = info.Principal.FindFirstValue(ClaimTypes.Email)
            };

            await userManager.CreateAsync(user);
          }

          await userManager.AddLoginAsync(user, info);
          await signInManager.SignInAsync(user, isPersistent: false);

          return LocalRedirect(returnUrl);
        }

        ViewBag.ErrorMessage = $"Email claim not received from: {info.LoginProvider}";
        return View("Error");
      }
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
      if (ModelState.IsValid)
      {
        var username = model.UsernameOrEmail;
        var user = await userManager.FindByEmailAsync(model.UsernameOrEmail);
        if (user != null)
        {
          username = user.UserName;
        }

        var result = await signInManager.PasswordSignInAsync(username, model.Password, model.RememberMe, false);

        if (result.Succeeded)
        {
          if (Url.IsLocalUrl(returnUrl))
          {
            return Redirect(returnUrl);
          }
          else
          {
            return RedirectToAction("index", "home");
          }
        }

        ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
      }

      return View(model);
    }

    /// <summary>
    /// Logout action
    /// </summary>
    /// <returns>Asynchronous logout to the home view</returns> 
    public async Task<IActionResult> Logout()
    {
      await signInManager.SignOutAsync();
      return RedirectToAction("index", "home");
    }
  }
}