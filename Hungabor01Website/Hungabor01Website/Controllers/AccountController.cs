using Hungabor01Website.Utilities;
using Hungabor01Website.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Hungabor01Website.Constants;
using Microsoft.Extensions.Logging;
using Hungabor01Website.Database;
using Hungabor01Website.Database.Entities;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Hungabor01Website.Controllers
{
  /// <summary>
  /// Manages the account related actions
  /// </summary>
  public class AccountController : Controller
  {
    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManager;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger logger;

    public AccountController(
      UserManager<IdentityUser> userManager,
      SignInManager<IdentityUser> signInManager,
      IServiceProvider serviceProvider,
      ILogger<AccountController> logger)
    {
      this.userManager = userManager;
      this.signInManager = signInManager;
      this.serviceProvider = serviceProvider;
      this.logger = logger;
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
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      var user = new IdentityUser
      {
        UserName = model.Username,
        Email = model.Email
      };

      var result = await userManager.CreateAsync(user, model.Password);

      if (result.Succeeded)
      {
        if (await SendConfirmationEmailAsync(user))
          ModelState.AddModelError(string.Empty, Strings.NotifyUserConfirmationEmailSent);
        else
          ModelState.AddModelError(string.Empty, Strings.NotifyUserConfirmationEmailSentError);

        var loginModel = new LoginViewModel { UsernameOrEmail = model.Email };

        return View("Login", loginModel);
      }
      else
      {
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError(string.Empty, error.Description);
          logger.LogWarning(EventIds.RegisterCreateUserError,
            string.Format(Strings.AccountError, error.Description, model.Username));
        }

        return View(model);
      }
    }

    private async Task<bool> SendConfirmationEmailAsync(IdentityUser user)
    {
      var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
      var confirmationLink =
        Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

      var emailSender = serviceProvider.GetService<IMessageSender>();

      var emailBody = string.Format(Strings.ConfirmationEmailBody, confirmationLink);

      return await emailSender.SendMessageAsync(
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
        return RedirectToAction("index", "home");
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
        await signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("index", "home");
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
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
          returnUrl = Url.Content("~/");
        }
        return LocalRedirect(returnUrl);
      }
      else
      {
        ModelState.AddModelError(string.Empty, Strings.InvalidLogin);
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
        "ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl, Source = source });

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
        return NavigateBackWithError(returnUrl, source, error);
      }

      var signInResult = await signInManager.ExternalLoginSignInAsync(
        info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

      if (signInResult.Succeeded)
      {
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
          return await RegisterAndLogin(returnUrl, info, email);
        }
      }
    }

    private IActionResult NavigateBackWithError(string returnUrl, string source, string error)
    {
      ModelState.AddModelError(string.Empty, error);
      logger.LogWarning(EventIds.ExternalLoginCallbackError, error);
      if (source == "Login")
      {
        return View(source, new LoginViewModel { ReturnUrl = returnUrl });
      }
      else
      {
        return View(source, new RegisterViewModel());
      }
    }

    private async Task<IActionResult> RegisterAndLogin(string returnUrl, ExternalLoginInfo info, string email)
    {
      var user = await userManager.FindByEmailAsync(email);

      if (user == null)
      {
        user = new IdentityUser
        {
          UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
          Email = info.Principal.FindFirstValue(ClaimTypes.Email)
        };

        var result = await userManager.CreateAsync(user);

        if (result.Succeeded)
        {
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

          return View("Register", model);
        }
      }

      await userManager.AddLoginAsync(user, info);
      await signInManager.SignInAsync(user, isPersistent: false);
      return LocalRedirect(returnUrl);
    }

    /// <summary>
    /// Get action of Forgot Password
    /// </summary>
    /// <returns>ForgotPassword view</returns>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
      return View();
    }

    /// <summary>
    /// Post method of Forgot Password
    /// </summary>
    /// <param name="model">ViewModel of ForgotPassword</param>
    /// <returns>View of Forgot Password</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      var user = await userManager.FindByEmailAsync(model.Email);

      if (user != null && await userManager.IsEmailConfirmedAsync(user))
      {
        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        var passwordResetLink = Url.Action("ResetPassword", "Account",
                new { email = model.Email, token = token }, Request.Scheme);

        var emailSender = serviceProvider.GetService<IMessageSender>();

        var emailBody = string.Format(Strings.PasswordResetEmailBody, passwordResetLink);

        await emailSender.SendMessageAsync(
          user.Email,
          string.Format(Strings.PasswordResetEmailSubject, Assembly.GetEntryAssembly().GetName().Name),
          emailBody);
      }

      ModelState.AddModelError(
        string.Empty, string.Format(Strings.ForgotPasswordSent, model.Email));
      return View(model);
    }

    /// <summary>
    /// Get method of Reset Password
    /// </summary>
    /// <param name="token">The token for the password reset</param>
    /// <param name="email">The email of the user, who resets the password</param>
    /// <returns>View of Forgot Password</returns>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string token, string email)
    {
      if ((token == null || email == null) && !signInManager.IsSignedIn(User))
      {
        return  RedirectToAction("index", "home");
      }

      var model = new ResetPasswordViewModel
      {
        Email = email,
        Token = token        
      };

      return View(model);
    }

    /// <summary>
    /// Post method of Forgot Password
    /// </summary>
    /// <param name="model">ViewModel of Forgot Password</param>
    /// <returns>The view of Forgot Password</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      IdentityUser user;
      if (signInManager.IsSignedIn(User))
      {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        user = await userManager.FindByIdAsync(userId);
        model.Email = user.Email;
        model.Token = await userManager.GeneratePasswordResetTokenAsync(user);
      }
      else
      {
        user = await userManager.FindByEmailAsync(model.Email);
      }

      if (user != null)
      {
        var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (!result.Succeeded)
        {
          foreach (var error in result.Errors)
          {
            ModelState.AddModelError(string.Empty, error.Description);
          }
          logger.LogWarning(
            EventIds.ResetPasswordError, string.Format(Strings.ResetPasswordError, model.Email, model.Token));
          return View(model);
        }
      }

      ModelState.AddModelError(
        string.Empty, string.Format(Strings.ResetPasswordNotification, model.Email));
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

    /// <summary>
    /// Get action of the EditAccount view
    /// </summary>
    /// <returns>EditAccount view</returns>
    [HttpGet]
    public IActionResult EditAccount()
    {      
      return View();
    }

    /// <summary>
    /// Proceses the changes of the user account
    /// </summary>
    /// <param name="model">EditAccount view model</param>
    /// <returns>EditAccount view</returns>
    [HttpPost]
    public async Task<IActionResult> EditAccount(EditAccountViewModel model)
    {
      if (ModelState.IsValid && model.ProfilePicture != null)
      {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);       

        using (var unitOfWork = serviceProvider.GetService<IUnitOfWork>())
        {
          var attachment = unitOfWork.Attachments.SingleOrDefault(
            x => x.User.Id == userId && x.Type == AttachmentType.ProfilePicture);
          if (attachment == null)
          {
            attachment = new Attachment()
            {
              User = await userManager.FindByIdAsync(userId),
              Type = AttachmentType.ProfilePicture,
              Filename = Path.GetFileNameWithoutExtension(model.ProfilePicture.FileName),
              Extension = Path.GetExtension(model.ProfilePicture.FileName),            
              Data = ConvertFileToBytes(model.ProfilePicture)
            };

            unitOfWork.Attachments.Add(attachment);
          }
          else
          {
            attachment.Filename = Path.GetFileNameWithoutExtension(model.ProfilePicture.FileName);
            attachment.Extension = Path.GetExtension(model.ProfilePicture.FileName);     
            attachment.Data = ConvertFileToBytes(model.ProfilePicture);
          }

          unitOfWork.Complete();
        }
      }
      return View();
    }

    private byte[] ConvertFileToBytes(IFormFile file)
    {      
      byte[] fileBytes = null;
      using (var ms = new MemoryStream())
      {
        file.CopyTo(ms);
        fileBytes = ms.ToArray();
      }
      return fileBytes;
    }
    
    /// <summary>
    /// Serves the uploaded image of the user or the default blankuser.jpg
    /// </summary>
    /// <returns>The image as file</returns>
    public async Task<FileResult> GetProfilePicture()
    {
      using (var unitOfWork = serviceProvider.GetService<IUnitOfWork>())
      {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId != null)
        {
          var attachment = unitOfWork.Attachments.SingleOrDefault(
            x => x.User.Id == userId && x.Type == AttachmentType.ProfilePicture);
          if (attachment != null)
          {
            return File(attachment.Data, "image/" + attachment.Extension);
          }
        }
      }

      var filePath = Path.Combine(
        Directory.GetCurrentDirectory(), "wwwroot", "images", "blankuser.jpg");

      var file = await System.IO.File.ReadAllBytesAsync(filePath);

      return File(file, "image/jpg");
    }   
    
    /// <summary>
    /// Removes the profile picture from the database for the current user
    /// </summary>
    /// <returns></returns>
    public IActionResult DeleteProfilePicture()
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

      using (var unitOfWork = serviceProvider.GetService<IUnitOfWork>())
      {
        var attachment = unitOfWork.Attachments.SingleOrDefault(
          x => x.User.Id == userId && x.Type == AttachmentType.ProfilePicture);

        unitOfWork.Attachments.Remove(attachment);

        unitOfWork.Complete();
      }   

      return View("EditAccount");
    }
  }
}