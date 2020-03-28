using Hungabor01Website.BusinessLogic.Enums;
using Hungabor01Website.Constants;
using Hungabor01Website.Constants.Strings;
using Hungabor01Website.Controllers;
using Hungabor01Website.DataAccess.Managers.Interfaces;
using Hungabor01Website.Database.Core;
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
    public class AccountControllerTests
    {
        private AccountController accountController;

        private readonly Mock<UserManager<ApplicationUser>> mockUserManager;
        private readonly Mock<SignInManager<ApplicationUser>> mockSignInManager;
        private readonly Mock<ILogger<AccountController>> mockLogger;
        private readonly Mock<IAccountManager> _mockManager;

        public AccountControllerTests()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object,
                null, null, null, null, null, null, null, null);

            var accessor = new Mock<IHttpContextAccessor>();
            var claims = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                mockUserManager.Object,
                accessor.Object,
                claims.Object,
                null, null, null, null);

            mockLogger = new Mock<ILogger<AccountController>>();
            _mockManager = new Mock<IAccountManager>();
        }

        private void CreateController()
        {
            accountController = new AccountController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockLogger.Object,
                _mockManager.Object);
        }

        [Fact]
        public void ForgotPassword_Get_ReturnView()
        {
            CreateController();

            var actionResult = accountController.ForgotPassword();

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task ForgotPassword_PostInvalidModel_ReturnView()
        {
            CreateController();

            accountController.ModelState.AddModelError(string.Empty, "InvalidModelState");

            var actionResult = await accountController.ForgotPassword(new ForgotPasswordViewModel());

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.ViewName);
            Assert.IsType<ForgotPasswordViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task ForgotPassword_CouldNotFindUser_ReturnViewWithMessage()
        {
            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult<ApplicationUser>(null));
                        
            CreateController();

            var model = new ForgotPasswordViewModel
            {
                Email = "test@test.com"
            };

            var actionResult = await accountController.ForgotPassword(model);

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.ViewName);
            Assert.IsType<ForgotPasswordViewModel>(viewResult.Model);
            Assert.Equal(string.Format(AccountStrings.ForgotPasswordSent, model.Email), viewResult.ViewData.ModelState[string.Empty].Errors[0].ErrorMessage);
            
        }

        [Fact]
        public async Task ForgotPassword_EmailNotConfirmned_ReturnViewWithMessage()
        {
            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new ApplicationUser()));

            mockUserManager.Setup(um => um.IsEmailConfirmedAsync(It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult(false));

            CreateController();

            var model = new ForgotPasswordViewModel
            {
                Email = "test@test.com"
            };

            var actionResult = await accountController.ForgotPassword(model);

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.ViewName);
            Assert.IsType<ForgotPasswordViewModel>(viewResult.Model);
            Assert.Equal(string.Format(AccountStrings.ForgotPasswordSent, model.Email), viewResult.ViewData.ModelState[string.Empty].Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task ForgotPassword_EmailNotSent_ReturnViewWithMessage()
        {
            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new ApplicationUser()));

            mockUserManager.Setup(um => um.IsEmailConfirmedAsync(It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult(true));

            mockUserManager.Setup(um => um.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult("testToken"));

            _mockManager.Setup(h => h.LogUserActionToDatabaseAsync(It.IsAny<ApplicationUser>(), It.IsAny<UserActionType>(), It.IsAny<string>())).Verifiable();
            _mockManager.Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            CreateController();

            var mockUrl = new Mock<IUrlHelper>();
            mockUrl.Setup(h => h.Action(It.IsAny<UrlActionContext>()))
                .Returns("testUrl");
            accountController.Url = mockUrl.Object;

            accountController.ControllerContext.HttpContext = new DefaultHttpContext();

            var model = new ForgotPasswordViewModel
            {
                Email = "test@test.com"
            };

            var actionResult = await accountController.ForgotPassword(model);

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.ViewName);
            Assert.IsType<ForgotPasswordViewModel>(viewResult.Model);
            Assert.Equal(string.Format(AccountStrings.ForgotPasswordSent, model.Email), viewResult.ViewData.ModelState[string.Empty].Errors[0].ErrorMessage);
            _mockManager.Verify(h => h.LogUserActionToDatabaseAsync(It.IsAny<ApplicationUser>(), It.IsAny<UserActionType>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ForgotPassword_EmailSent_ReturnViewWithMessage()
        {
            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new ApplicationUser()));

            mockUserManager.Setup(um => um.IsEmailConfirmedAsync(It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult(true));

            mockUserManager.Setup(um => um.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult("testToken"));

            _mockManager.Setup(h => h.LogUserActionToDatabaseAsync(It.IsAny<ApplicationUser>(), It.IsAny<UserActionType>(), It.IsAny<string>())).Verifiable();
            _mockManager.Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            CreateController();

            var mockUrl = new Mock<IUrlHelper>();
            mockUrl.Setup(h => h.Action(It.IsAny<UrlActionContext>()))
                .Returns("testUrl");
            accountController.Url = mockUrl.Object;

            accountController.ControllerContext.HttpContext = new DefaultHttpContext();

            var model = new ForgotPasswordViewModel
            {
                Email = "test@test.com"
            };

            var actionResult = await accountController.ForgotPassword(model);

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.ViewName);
            Assert.IsType<ForgotPasswordViewModel>(viewResult.Model);
            Assert.Equal(string.Format(AccountStrings.ForgotPasswordSent, model.Email), viewResult.ViewData.ModelState[string.Empty].Errors[0].ErrorMessage);
            _mockManager.Verify(h => h.LogUserActionToDatabaseAsync(It.IsAny<ApplicationUser>(), It.IsAny<UserActionType>(), It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData("", "testEmail")]
        [InlineData("testtoken", "")]
        public void ResetPassword_GetInvalidParameters_RedirectToHome(string token, string email)
        {
            mockSignInManager.Setup(si => si.IsSignedIn(It.IsAny<ClaimsPrincipal>())).Returns(false);

            CreateController();

            var actionResult = accountController.ResetPassword(token, email);

            var viewResult = Assert.IsType<RedirectToActionResult>(actionResult);
            Assert.Equal("Home", viewResult.ControllerName);
            Assert.Equal("Index", viewResult.ActionName);
        }

        [Theory]
        [InlineData("", "testEmail")]
        [InlineData("testToken", "")]
        public void ResetPassword_GetInvalidParametersButSignedIn_ReturnView(string token, string email)
        {
            mockSignInManager.Setup(si => si.IsSignedIn(It.IsAny<ClaimsPrincipal>())).Returns(true);

            CreateController();

            var actionResult = accountController.ResetPassword(token, email);

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.ViewName);
            Assert.IsType<ResetPasswordViewModel>(viewResult.Model);
        }

        [Fact]
        public void ResetPassword_GetValidParametersNotSignedIn_ReturnView()
        {
            mockSignInManager.Setup(si => si.IsSignedIn(It.IsAny<ClaimsPrincipal>())).Returns(false);

            CreateController();

            var actionResult = accountController.ResetPassword("testToken", "testEmail");

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.ViewName);
            Assert.IsType<ResetPasswordViewModel>(viewResult.Model);
        }
    }
}
