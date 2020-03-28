using Hungabor01Website.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Hungabor01Website.Constants;
using Microsoft.Extensions.Logging;
using Hungabor01Website.Database.Core;
using Hungabor01Website.BusinessLogic.Enums;
using Hungabor01Website.DataAccess.Managers.Interfaces;
using Hungabor01Website.Constants.Strings;
using System.Reflection;

namespace Hungabor01Website.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly IRegistrationManager _manager;

        public RegisterController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterController> logger,
            IRegistrationManager manager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _manager = manager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

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

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _manager.LogUserActionToDatabaseAsync(user, UserActionType.Registration);

                var isSent = await SendConfirmationEmailAsync(user);
                if (isSent)
                {
                    ModelState.AddModelError(string.Empty, RegistrationStrings.NotifyUserConfirmationEmailSent);
                    await _manager.LogUserActionToDatabaseAsync(user, UserActionType.ConfirmationEmailSent);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, RegistrationStrings.NotifyUserConfirmationEmailSentError);
                }
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    _logger.LogWarning(EventIds.RegisterCreateUserError, string.Format(Strings.AccountError, error.Description, model.Username));
                }        
            }

            return View(model);
        }

        private async Task<bool> SendConfirmationEmailAsync(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(
                "ConfirmEmail", "Register",
                new { userId = user.Id, token = token },
                Request.Scheme
            );
            var emailBody = string.Format(RegistrationStrings.ConfirmationEmailBody, confirmationLink);
            var subject = string.Format(RegistrationStrings.ConfirmationEmailSubject, Assembly.GetEntryAssembly().GetName().Name);

            return await _manager.SendEmailAsync(user.Email, subject, emailBody);
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

            if (result.Succeeded)
            {
                await _manager.LogUserActionToDatabaseAsync(user, UserActionType.EmailConfirmed);
                await _signInManager.SignInAsync(user, isPersistent: false);
                await _manager.LogUserActionToDatabaseAsync(user, UserActionType.Login, LoginStrings.LoginBuiltIn);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = RegistrationStrings.EmailConfirmationError;
                _logger.LogWarning(EventIds.ConfirmEmailCannotConfirmError, string.Format(Strings.AccountError, RegistrationStrings.EmailConfirmationError, userId));
                return View("Error");
            }
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