using BusinessLogic.ControllerManagers.Interfaces;
using Common.Enums;
using Common.Strings;
using Database.Core;
using Hungabor01Website.Controllers;
using Hungabor01Website.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Hungabor01Website.Tests.Controllers
{
    public class ProfileControllerTests
    {
        private ProfileController _profileController;

        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private readonly Mock<IAccountControllersManager> _mockManager;
        private readonly Mock<ILogger<ProfileController>> _mockLogger;
        
        public ProfileControllerTests()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object,
                null, null, null, null, null, null, null, null);

            var accessor = new Mock<IHttpContextAccessor>();
            var claims = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                _mockUserManager.Object,
                accessor.Object,
                claims.Object,
                null, null, null, null);

            _mockManager = new Mock<IAccountControllersManager>();

            _mockLogger = new Mock<ILogger<ProfileController>>();
        }

        private void CreateController()
        {
            _profileController = new ProfileController(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockManager.Object,
                _mockLogger.Object);
        }

        [Fact]
        public void ForgotPassword_Get_ReturnView()
        {
            CreateController();

            var actionResult = _profileController.ForgotPassword();

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task ForgotPassword_PostInvalidModel_ReturnView()
        {
            CreateController();

            _profileController.ModelState.AddModelError(string.Empty, "InvalidModelState");

            var actionResult = await _profileController.ForgotPassword(new ForgotPasswordViewModel());

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.ViewName);
            Assert.IsType<ForgotPasswordViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task ForgotPassword_CouldNotFindUser_ReturnViewWithMessage()
        {
            _mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult<ApplicationUser>(null));
                        
            CreateController();

            var model = new ForgotPasswordViewModel
            {
                Email = "test@test.com"
            };

            var actionResult = await _profileController.ForgotPassword(model);

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.ViewName);
            Assert.IsType<ForgotPasswordViewModel>(viewResult.Model);
            Assert.Equal(string.Format(ProfileStrings.ForgotPasswordSent, model.Email), viewResult.ViewData.ModelState[string.Empty].Errors[0].ErrorMessage);
            
        }

        [Fact]
        public async Task ForgotPassword_EmailNotConfirmned_ReturnViewWithMessage()
        {
            _mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new ApplicationUser()));

            _mockUserManager.Setup(um => um.IsEmailConfirmedAsync(It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult(false));

            CreateController();

            var model = new ForgotPasswordViewModel
            {
                Email = "test@test.com"
            };

            var actionResult = await _profileController.ForgotPassword(model);

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.ViewName);
            Assert.IsType<ForgotPasswordViewModel>(viewResult.Model);
            Assert.Equal(string.Format(ProfileStrings.ForgotPasswordSent, model.Email), viewResult.ViewData.ModelState[string.Empty].Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task ForgotPassword_EmailNotSent_ReturnViewWithMessage()
        {
            _mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new ApplicationUser()));

            _mockUserManager.Setup(um => um.IsEmailConfirmedAsync(It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult(true));

            _mockUserManager.Setup(um => um.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult("testToken"));

            _mockManager.Setup(h => h.LogUserActionToDatabaseAsync(It.IsAny<ApplicationUser>(), It.IsAny<UserActionType>(), It.IsAny<string>())).Verifiable();
            _mockManager.Setup(m => m.SendForgotPasswordEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            CreateController();

            var mockUrl = new Mock<IUrlHelper>();
            mockUrl.Setup(h => h.Action(It.IsAny<UrlActionContext>()))
                .Returns("testUrl");
            _profileController.Url = mockUrl.Object;

            _profileController.ControllerContext.HttpContext = new DefaultHttpContext();

            var model = new ForgotPasswordViewModel
            {
                Email = "test@test.com"
            };

            var actionResult = await _profileController.ForgotPassword(model);

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.ViewName);
            Assert.IsType<ForgotPasswordViewModel>(viewResult.Model);
            Assert.Equal(string.Format(ProfileStrings.ForgotPasswordSent, model.Email), viewResult.ViewData.ModelState[string.Empty].Errors[0].ErrorMessage);
            _mockManager.Verify(h => h.LogUserActionToDatabaseAsync(It.IsAny<ApplicationUser>(), It.IsAny<UserActionType>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ForgotPassword_EmailSent_ReturnViewWithMessage()
        {
            _mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new ApplicationUser()));

            _mockUserManager.Setup(um => um.IsEmailConfirmedAsync(It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult(true));

            _mockUserManager.Setup(um => um.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult("testToken"));

            _mockManager.Setup(h => h.LogUserActionToDatabaseAsync(It.IsAny<ApplicationUser>(), It.IsAny<UserActionType>(), It.IsAny<string>())).Verifiable();
            _mockManager.Setup(m => m.SendForgotPasswordEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            CreateController();

            var mockUrl = new Mock<IUrlHelper>();
            mockUrl.Setup(h => h.Action(It.IsAny<UrlActionContext>()))
                .Returns("testUrl");
            _profileController.Url = mockUrl.Object;

            _profileController.ControllerContext.HttpContext = new DefaultHttpContext();

            var model = new ForgotPasswordViewModel
            {
                Email = "test@test.com"
            };

            var actionResult = await _profileController.ForgotPassword(model);

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.ViewName);
            Assert.IsType<ForgotPasswordViewModel>(viewResult.Model);
            Assert.Equal(string.Format(ProfileStrings.ForgotPasswordSent, model.Email), viewResult.ViewData.ModelState[string.Empty].Errors[0].ErrorMessage);
            _mockManager.Verify(h => h.LogUserActionToDatabaseAsync(It.IsAny<ApplicationUser>(), It.IsAny<UserActionType>(), It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData("", "testEmail")]
        [InlineData("testtoken", "")]
        public void ResetPassword_GetInvalidParameters_RedirectToHome(string token, string email)
        {
            _mockSignInManager.Setup(si => si.IsSignedIn(It.IsAny<ClaimsPrincipal>())).Returns(false);

            CreateController();

            var actionResult = _profileController.ResetPassword(token, email);

            var viewResult = Assert.IsType<RedirectToActionResult>(actionResult);
            Assert.Equal("Home", viewResult.ControllerName);
            Assert.Equal("Index", viewResult.ActionName);
        }

        [Theory]
        [InlineData("", "testEmail")]
        [InlineData("testToken", "")]
        public void ResetPassword_GetInvalidParametersButSignedIn_ReturnView(string token, string email)
        {
            _mockSignInManager.Setup(si => si.IsSignedIn(It.IsAny<ClaimsPrincipal>())).Returns(true);

            CreateController();

            var actionResult = _profileController.ResetPassword(token, email);

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.ViewName);
            Assert.IsType<ResetPasswordViewModel>(viewResult.Model);
        }

        [Fact]
        public void ResetPassword_GetValidParametersNotSignedIn_ReturnView()
        {
            _mockSignInManager.Setup(si => si.IsSignedIn(It.IsAny<ClaimsPrincipal>())).Returns(false);

            CreateController();

            var actionResult = _profileController.ResetPassword("testToken", "testEmail");

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.ViewName);
            Assert.IsType<ResetPasswordViewModel>(viewResult.Model);
        }
    }
}
