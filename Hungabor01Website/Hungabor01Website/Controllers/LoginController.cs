using Hungabor01Website.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Database.Core;
using BusinessLogic.ControllerManagers.Interfaces;
using Common;
using Common.Strings;
using Common.Enums;

namespace Hungabor01Website.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAccountControllersManager _manager;
        private readonly ILogger _logger;

        public LoginController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IAccountControllersManager manager,
            ILogger<LoginController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _manager = manager;
            _logger = logger;
        }

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

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetUserFromModel(model);

            if (user == null)
            {                
                _logger.LogWarning(EventIds.LoginError, LoginStrings.LoginUsernameError);
                ModelState.AddModelError(string.Empty, LoginStrings.InvalidLogin);
                return View(model);
            }

            if (!user.EmailConfirmed && (await _userManager.CheckPasswordAsync(user, model.Password)))
            {
                ModelState.AddModelError(string.Empty, LoginStrings.EmailNotConfirmed);
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, LoginStrings.InvalidLogin);
                _logger.LogWarning(EventIds.LoginError, LoginStrings.LoginPasswordError);
                return View(model);
            }

            await _manager.LogUserActionToDatabaseAsync(user, UserActionType.Login, LoginStrings.LoginBuiltIn);
            
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = Url.Content("~/");
            }

            return LocalRedirect(returnUrl);
        }

        private async Task<ApplicationUser> GetUserFromModel(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.UsernameOrEmail);

            if (user == null)
            {
                user = await _userManager.FindByNameAsync(model.UsernameOrEmail);
            }

            return user;
        }

        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl, string source)
        {
            var redirectUrl = Url.Action(
                "ExternalLoginCallback", "Login",
                new { ReturnUrl = returnUrl, Source = source });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }
 
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string source = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            var info = await CheckExternalErrorsAsync(remoteError);
      
            if (info == null)
            {
                if (source == "Login")
                {
                    return View("Login", new LoginViewModel { ReturnUrl = returnUrl });
                }
                else
                {
                    return RedirectToAction("Register" , "Register", new RegistrationViewModel());
                }
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {                
                var user = await _userManager.FindByEmailAsync(email);
                await _manager.LogUserActionToDatabaseAsync(user, UserActionType.Login, string.Format(LoginStrings.LoginExternal, info.LoginProvider));
                return LocalRedirect(returnUrl);
            }

            if (email == null)
            {
                ViewBag.ErrorMessage = string.Format(LoginStrings.EmailClaimError, info.LoginProvider);
                _logger.LogWarning(EventIds.ExternalLoginCallbackEmailError, string.Format(LoginStrings.ExternalLoginCallbackEmailError, info.LoginProvider));
                return View("Error");
            }

            return await RegisterAndLoginUserAsync(returnUrl, info, email);
        }

        private async Task<ExternalLoginInfo> CheckExternalErrorsAsync(string remoteError)
        {
            string error = null;
            ExternalLoginInfo info = null;

            if (remoteError != null)
            {
                error = string.Format(LoginStrings.ExternalProviderError, remoteError);
            }
            else
            {
                info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    error = LoginStrings.ExternalLoginError;
                }
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                _logger.LogWarning(EventIds.ExternalLoginCallbackError, error);
                ModelState.AddModelError(string.Empty, error);
            }

            return info;
        }

        private async Task<IActionResult> RegisterAndLoginUserAsync(string returnUrl, ExternalLoginInfo info, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email
                };

                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    await _manager.LogUserActionToDatabaseAsync(user, UserActionType.Registration);
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    await _userManager.ConfirmEmailAsync(user, token);
                }
                else
                {
                    var model = new RegistrationViewModel();
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        _logger.LogWarning(EventIds.RegisterExternalCreateUserError, string.Format(Strings.AccountError, error.Description, user.UserName));
                    }

                    return RedirectToAction("Register", "Register", model);
                }
            }

            await _userManager.AddLoginAsync(user, info);
            await _signInManager.SignInAsync(user, isPersistent: false);
            await _manager.LogUserActionToDatabaseAsync(user, UserActionType.Login, string.Format(LoginStrings.LoginExternal, info.LoginProvider));
            return LocalRedirect(returnUrl);
        }
    }
}