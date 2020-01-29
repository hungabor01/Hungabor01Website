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

namespace Hungabor01Website.Controllers
{
  /// <summary>
  /// Manages the account related actions
  /// </summary>
  public class AccountController : Controller
  {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger logger;
    private readonly ILoginRegistrationAccountHelper helper;
    
    public AccountController(
      UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager,
      IServiceProvider serviceProvider,
      ILogger<AccountController> logger,
      ILoginRegistrationAccountHelper helper)
    {
      this.userManager = userManager;
      this.signInManager = signInManager;
      this.serviceProvider = serviceProvider;
      this.helper = helper;
      this.logger = logger;
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

        var result = emailSender.SendMessage(
          user.Email,
          string.Format(Strings.PasswordResetEmailSubject, Assembly.GetEntryAssembly().GetName().Name),
          emailBody);

        if (result)
        {
          helper.LogUserActionToDatabase(user, UserActionType.ForogotPasswordEmailSent);
        }
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
        return  RedirectToAction("Index", "Home");
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

      ApplicationUser user;
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
        if (result.Succeeded)
        {
          helper.LogUserActionToDatabase(user, UserActionType.PasswordChanged);
        }
        else
        {
          foreach (var error in result.Errors)
          { 
            logger.LogWarning(
              EventIds.ResetPasswordError, string.Format(Strings.ResetPasswordError, model.Email, model.Token, error.Description));
          }          

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
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      var user = await userManager.FindByIdAsync(userId);

      helper.LogUserActionToDatabase(user, UserActionType.Logout);      

      await signInManager.SignOutAsync();
      return RedirectToAction("Index", "Home");
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
        var user = await userManager.FindByIdAsync(userId);

        using (var unitOfWork = serviceProvider.GetService<IUnitOfWork>())
        {
          unitOfWork.AttachmentRepository.ChangeProfilePicture(user, model.ProfilePicture);
          unitOfWork.Complete();
        }

        helper.LogUserActionToDatabase(user, UserActionType.ProfilePictureChanged);
      }

      return View();
    }
    
    /// <summary>
    /// Serves the uploaded image of the user or the default blankuser.jpg
    /// </summary>
    /// <returns>The image as file</returns>
    [AllowAnonymous]
    public async Task<FileResult> GetProfilePicture()
    {
      using (var unitOfWork = serviceProvider.GetService<IUnitOfWork>())
      {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await userManager.FindByIdAsync(userId);

        var fileTuple = unitOfWork.AttachmentRepository.GetProfilePicture(user);

        if (fileTuple.HasValue)
        {
          return File(fileTuple.Value.Data, "image/" + fileTuple.Value.Extension);
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
    /// <returns>The EditAccount view with the result of the delete</returns>
    public async Task<IActionResult> DeleteProfilePictureAsync()
    {
      using (var unitOfWork = serviceProvider.GetService<IUnitOfWork>())
      {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await userManager.FindByIdAsync(userId);

        var result = unitOfWork.AttachmentRepository.DeleteProfilePicture(user);
        if (result)
        {
          ModelState.AddModelError(string.Empty, Strings.ProfilePictureDeleted);
          helper.LogUserActionToDatabase(
            user, UserActionType.ProfilePictureDeleted);
        }
        else
        {
          ModelState.AddModelError(string.Empty, Strings.ProfilePictureDeletedError);
        }
        unitOfWork.Complete();
      }   

      return View("EditAccount");
    }  
  }
}