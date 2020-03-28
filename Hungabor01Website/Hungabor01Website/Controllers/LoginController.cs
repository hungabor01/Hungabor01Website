using Hungabor01Website.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Hungabor01Website.Constants;
using Microsoft.Extensions.Logging;
using Hungabor01Website.Database.Core;
using Hungabor01Website.DataAccess.Managers.Interfaces;
using Hungabor01Website.BusinessLogic.Enums;
using Hungabor01Website.Constants.Strings;

namespace Hungabor01Website.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly ILoginManager _manager;

        public LoginController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<LoginController> logger,
            ILoginManager manager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _manager = manager;
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

            var user = await _userManager.FindByEmailAsync(model.UsernameOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(model.UsernameOrEmail);
            }

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, LoginStrings.InvalidLogin);
                _logger.LogWarning(EventIds.LoginError, LoginStrings.LoginUsernameError);
                return View(model);
            }

            if (!user.EmailConfirmed && (await _userManager.CheckPasswordAsync(user, model.Password)))
            {
                ModelState.AddModelError(string.Empty, LoginStrings.EmailNotConfirmed);
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                await _manager.LogUserActionToDatabaseAsync(user, UserActionType.Login, LoginStrings.LoginBuiltIn);
                if (string.IsNullOrWhiteSpace(returnUrl))
                {
                    returnUrl = Url.Content("~/");
                }
                return LocalRedirect(returnUrl);
            }
            else
            {
                ModelState.AddModelError(string.Empty, LoginStrings.InvalidLogin);
                _logger.LogWarning(EventIds.LoginError, LoginStrings.LoginPasswordError);
                return View(model);
            }
        }

        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl, string source)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Login", new { ReturnUrl = returnUrl, Source = source });
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
                    return RedirectToAction("Register" , "Register", new RegisterViewModel());
                }
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var user = await _userManager.FindByEmailAsync(email);
                await _manager.LogUserActionToDatabaseAsync(user, UserActionType.Login, string.Format(LoginStrings.LoginExternal, info.LoginProvider));
                return LocalRedirect(returnUrl);
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email == null)
                {
                    ViewBag.ErrorMessage = string.Format(LoginStrings.EmailClaimError, info.LoginProvider);
                    _logger.LogWarning(EventIds.ExternalLoginCallbackEmailError, string.Format(LoginStrings.ExternalLoginCallbackEmailError, info.LoginProvider));
                    return View("Error");
                }
                else
                {
                    return await RegisterAndLoginAsync(returnUrl, info, email);
                }
            }
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
                ModelState.AddModelError(string.Empty, error);
                _logger.LogWarning(EventIds.ExternalLoginCallbackError, error);
            }

            return info;
        }

        private async Task<IActionResult> RegisterAndLoginAsync(string returnUrl, ExternalLoginInfo info, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email)
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
                    var model = new RegisterViewModel();
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