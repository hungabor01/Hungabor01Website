using Hungabor01Website.Utilities.Interfaces;
using Hungabor01Website.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Hungabor01Website.Constants;
using Microsoft.Extensions.Logging;
using Hungabor01Website.Database.Entities;

namespace Hungabor01Website.Controllers
{
  /// <summary>
  /// Manages the registration processes
  /// </summary>
  public class RegisterController : Controller
  {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger logger;
    private readonly ILoginRegistrationAccountHelper helper;

    public RegisterController(
      UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager,
      IServiceProvider serviceProvider,
      ILogger<RegisterController> logger,
      ILoginRegistrationAccountHelper helper)
    {
      this.userManager = userManager;
      this.signInManager = signInManager;
      this.serviceProvider = serviceProvider;
      this.logger = logger;
      this.helper = helper;
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
    /// <returns>Asynchronous registration, redirects to the login view</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      var user = new ApplicationUser
      {
        UserName = model.Username,
        Email = model.Email
      };

      var result = await userManager.CreateAsync(user, model.Password);

      if (result.Succeeded)
      {
        helper.LogUserActionToDatabase(user, UserActionType.Registration);

        var isSent = await SendConfirmationEmailAsync(user);
        if (isSent)
        {
          ModelState.AddModelError(string.Empty, Strings.NotifyUserConfirmationEmailSent);
          helper.LogUserActionToDatabase(user, UserActionType.ConfirmationEmailSent);
        }
        else
        {
          ModelState.AddModelError(string.Empty, Strings.NotifyUserConfirmationEmailSentError);
        }
      }
      else
      {
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError(string.Empty, error.Description);
          logger.LogWarning(EventIds.RegisterCreateUserError,
            string.Format(Strings.AccountError, error.Description, model.Username));
        }        
      }

      return View(model);
    }

    private async Task<bool> SendConfirmationEmailAsync(ApplicationUser user)
    {
      var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
      var confirmationLink =
        Url.Action("ConfirmEmail", "Register", new { userId = user.Id, token = token }, Request.Scheme);

      var emailSender = serviceProvider.GetService<IMessageSender>();

      var emailBody = string.Format(Strings.ConfirmationEmailBody, confirmationLink);

      return emailSender.SendMessage(
        user.Email,
        string.Format(Strings.ConfirmationEmailSubject, Assembly.GetEntryAssembly().GetName().Name),
        emailBody
      );
    }

    /// <summary>
    /// Confirm the email address via the token
    /// </summary>
    /// <param name="userId">The user id of the user</param>
    /// <param name="token">The previously generated token</param>
    /// <returns>Redirects to home page</returns>
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
      if (userId == null || token == null)
      {
        return RedirectToAction("Index", "Home");
      }

      var user = await userManager.FindByIdAsync(userId);

      if (user == null)
      {
        ViewBag.ErrorMessage = Strings.EmailConfirmationError;
        logger.LogWarning(EventIds.ConfirmEmailUserIdNullError,
          string.Format(Strings.AccountError, Strings.EmailConfirmationError, userId));
        return View("Error");
      }

      var result = await userManager.ConfirmEmailAsync(user, token);

      if (result.Succeeded)
      {
        helper.LogUserActionToDatabase(user, UserActionType.EmailConfirmed);
        await signInManager.SignInAsync(user, isPersistent: false);
        helper.LogUserActionToDatabase(user, UserActionType.Login, Strings.LoginBuiltIn);
        return RedirectToAction("Index", "Home");
      }
      else
      {
        ViewBag.ErrorMessage = Strings.EmailConfirmationError;
        logger.LogWarning(EventIds.ConfirmEmailCannotConfirmError,
          string.Format(Strings.AccountError, Strings.EmailConfirmationError, userId));
        return View("Error");
      }
    }

    /// <summary>
    /// Checks the username on the client side
    /// </summary>
    /// <param name="username">The username to check</param>
    /// <returns>Is the username taken or not</returns>
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
        return Json(string.Format(Strings.UsernameIsTaken, username));
      }
    }

    /// <summary>
    /// Checks the email on the client side
    /// </summary>
    /// <param name="email">The email to check</param>
    /// <returns>Is the email taken or not</returns>
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
        return Json(string.Format(Strings.EmailIsTaken, email));
      }
    }
  }
}