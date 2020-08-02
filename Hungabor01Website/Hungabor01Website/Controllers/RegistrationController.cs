using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using BusinessLogic.ControllerManagers.Interfaces;
using Database.Core;
using Hungabor01Website.ViewModels;
using Common;
using Common.Strings;
using Common.Enums;

namespace Hungabor01Website.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAccountControllersManager _manager;
        private readonly ILogger _logger;
        
        public RegistrationController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IAccountControllersManager manager,
            ILogger<RegistrationController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _manager = manager;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registration(RegistrationViewModel model)
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

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogWarning(EventIds.RegistrationCreateUserError, string.Format(Strings.AccountError, error.Description, model.Username));
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            await _manager.LogUserActionToDatabaseAsync(user, UserActionType.Registration);

            var confirmationLink = await GenerateConfirmationLink(user);
            var isSent = await _manager.SendConfirmationEmailAsync(user, confirmationLink);

            if (!isSent)
            {
                ModelState.AddModelError(string.Empty, RegistrationStrings.NotifyUserConfirmationEmailSentError);
                return View(model);
            }

            await _manager.LogUserActionToDatabaseAsync(user, UserActionType.ConfirmationEmailSent);

            ModelState.AddModelError(string.Empty, RegistrationStrings.NotifyUserConfirmationEmailSent);
            return View(model);
        }

        private async Task<string> GenerateConfirmationLink(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return Url.Action(
                "ConfirmEmail", "Registration",
                new { userId = user.Id, token = token },
                Request.Scheme
            );
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = RegistrationStrings.EmailConfirmationError;
                _logger.LogWarning(EventIds.ConfirmEmailUserIdNullError, string.Format(Strings.AccountError, RegistrationStrings.EmailConfirmationError, userId));
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                ViewBag.ErrorMessage = RegistrationStrings.EmailConfirmationError;
                _logger.LogWarning(EventIds.ConfirmEmailCannotConfirmError, string.Format(Strings.AccountError, RegistrationStrings.EmailConfirmationError, userId));
                return View("Error");
            }

            await _manager.LogUserActionToDatabaseAsync(user, UserActionType.EmailConfirmed);

            await _signInManager.SignInAsync(user, isPersistent: false);

            await _manager.LogUserActionToDatabaseAsync(user, UserActionType.Login, LoginStrings.LoginBuiltIn);

            return RedirectToAction("Index", "Home");
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsUsernameInUse(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json(string.Format(RegistrationStrings.UsernameIsTaken, username));
            }
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json(string.Format(RegistrationStrings.EmailIsTaken, email));
            }
        }
    }
}