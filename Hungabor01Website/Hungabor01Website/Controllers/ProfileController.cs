using Hungabor01Website.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.IO;
using Database.Core;
using BusinessLogic.ControllerManagers.Interfaces;
using Common;
using Common.Strings;
using Common.Enums;

namespace Hungabor01Website.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAccountControllersManager _manager;
        private readonly ILogger _logger;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IAccountControllersManager manager,
            ILogger<ProfileController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _manager = manager;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null && await _userManager.IsEmailConfirmedAsync(user))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResetLink = Url.Action("ResetPassword", "Profile", new { email = model.Email, token = token }, Request.Scheme);
                var result = await _manager.SendForgotPasswordEmailAsync(user, passwordResetLink);

                if (result)
                {
                    await _manager.LogUserActionToDatabaseAsync(user, UserActionType.ForgotPasswordEmailSent);
                }
            }
            else
            {
                _logger.LogWarning(EventIds.ForgotPasswordWrongEmail, string.Format(ProfileStrings.ForgotPasswordWrongEmail, model.Email));
            }

            ModelState.AddModelError(string.Empty, string.Format(ProfileStrings.ForgotPasswordSent, model.Email));
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if ((string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(email)) && !_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetLoggedInUserAsync();
            if (user != null)
            {
                model.Email = user.Email;
                model.Token = await _userManager.GeneratePasswordResetTokenAsync(user);
            }
            else
            {
                user = await _userManager.FindByEmailAsync(model.Email);
            }

            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    await _manager.LogUserActionToDatabaseAsync(user, UserActionType.PasswordChanged);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogWarning(EventIds.ResetPasswordError, string.Format(ProfileStrings.ResetPasswordError, model.Email, model.Token, error.Description));
                    }

                    return View(model);
                }
            }
            else
            {
                _logger.LogWarning(EventIds.ResetPasswordWrongUser, ProfileStrings.ResetPasswordWrongUser);
            }

            ModelState.AddModelError(string.Empty, string.Format(ProfileStrings.ResetPasswordNotification, model.Email));
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            var user = await GetLoggedInUserAsync();

            if (user != null)
            {
                await _signInManager.SignOutAsync();
                await _manager.LogUserActionToDatabaseAsync(user, UserActionType.Logout);                
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult EditAccount()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditAccount(EditAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await GetLoggedInUserAsync();
            if (user == null)
            {
                return View();
            }

            if (model.ProfilePicture != null)
            {
                await _manager.UploadProfilePictureAsync(user, model.ProfilePicture);
                await _manager.LogUserActionToDatabaseAsync(user, UserActionType.ProfilePictureChanged);
            }

            return View();
        }

        [AllowAnonymous]
        public async Task<FileResult> GetProfilePicture()
        {
            var user = await GetLoggedInUserAsync();

            if (user != null)
            {
                var fileTuple = await _manager.GetProfilePictureAsync(user);
                if (fileTuple.HasValue)
                {
                    return File(fileTuple.Value.Data, "image/" + fileTuple.Value.Extension);
                }
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "blankuser.jpg");
            var file = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(file, "image/jpg");
        }

        public async Task<IActionResult> DeleteProfilePictureAsync()
        {
            var user = await GetLoggedInUserAsync();

            if (user != null)
            {
                var result = await _manager.DeleteProfilePictureAsync(user);
                if (result)
                {
                    await _manager.LogUserActionToDatabaseAsync(user, UserActionType.ProfilePictureDeleted);
                    ModelState.AddModelError(string.Empty, ProfileStrings.ProfilePictureDeleted);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, ProfileStrings.ProfilePictureDeletedError);
                }
            }

            return View("EditAccount");
        }

        private async Task<ApplicationUser> GetLoggedInUserAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _userManager.FindByIdAsync(userId);
        }
    }
}