using Hungabor01Website.BusinessLogic.Enums;
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
using System.Threading.Tasks;
using Xunit;

namespace Hungabor01Website.Tests.Controllers
{
    public class RegisterControllerTests
    {
        private RegisterController registerController;

        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private readonly Mock<ILogger<RegisterController>> _mockLogger;
        private readonly Mock<IRegistrationManager> _mockManager;

        public RegisterControllerTests()
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

            _mockLogger = new Mock<ILogger<RegisterController>>();
            _mockManager = new Mock<IRegistrationManager>();
        }

        private void CreateController()
        {
            registerController = new RegisterController(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockLogger.Object,
                _mockManager.Object);
        }

        [Fact]
        public void Register_Get_ReturnView()
        {
            CreateController();

            var actionResult = registerController.Register();

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task Register_PostInvalidModel_ReturnView()
        {
            CreateController();

            registerController.ModelState.AddModelError(string.Empty, "InvalidModelState");

            var actionResult = await registerController.Register(new RegisterViewModel());

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.ViewName);
            Assert.IsType<RegisterViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task Register_CreateUserFails_ReturnViewWithErrors()
        {
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "testCode", Description = "testDescription" })));

            CreateController();

            var actionResult = await registerController.Register(new RegisterViewModel());

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.ViewName);
            Assert.IsType<RegisterViewModel>(viewResult.Model);
            Assert.Equal("testDescription", viewResult.ViewData.ModelState[string.Empty].Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task Register_UserCreatedConfirmEmailNotSent_ReturnViewWithErrors()
        {
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success));

            _mockManager.Setup(m => m.LogUserActionToDatabaseAsync(It.IsAny<ApplicationUser>(), It.IsAny<UserActionType>(), It.IsAny<string>())).Verifiable();
            _mockManager.Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            CreateController();

            var mockUrl = new Mock<IUrlHelper>();
            mockUrl.Setup(h => h.Action(It.IsAny<UrlActionContext>()))
                .Returns("testUrl");
            registerController.Url = mockUrl.Object;

            registerController.ControllerContext.HttpContext = new DefaultHttpContext();

            var actionResult = await registerController.Register(new RegisterViewModel());

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.ViewName);
            Assert.IsType<RegisterViewModel>(viewResult.Model);
            Assert.Equal(RegistrationStrings.NotifyUserConfirmationEmailSentError, viewResult.ViewData.ModelState[string.Empty].Errors[0].ErrorMessage);
            _mockManager.Verify(h => h.LogUserActionToDatabaseAsync(It.IsAny<ApplicationUser>(), It.IsAny<UserActionType>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Register_UserCreatedConfirmEmailSent_ReturnViewWithNotification()
        {
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
              .Returns(Task.FromResult(IdentityResult.Success));

            _mockManager.Setup(h => h.LogUserActionToDatabaseAsync(It.IsAny<ApplicationUser>(), It.IsAny<UserActionType>(), It.IsAny<string>())).Verifiable();
            _mockManager.Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            CreateController();

            var mockUrl = new Mock<IUrlHelper>();
            mockUrl.Setup(h => h.Action(It.IsAny<UrlActionContext>()))
                .Returns("testUrl");
            registerController.Url = mockUrl.Object;

            registerController.ControllerContext.HttpContext = new DefaultHttpContext();

            var actionResult = await registerController.Register(new RegisterViewModel());

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.ViewName);
            Assert.IsType<RegisterViewModel>(viewResult.Model);
            Assert.Equal(RegistrationStrings.NotifyUserConfirmationEmailSent, viewResult.ViewData.ModelState[string.Empty].Errors[0].ErrorMessage);
            _mockManager.Verify(h => h.LogUserActionToDatabaseAsync(It.IsAny<ApplicationUser>(), It.IsAny<UserActionType>(), It.IsAny<string>()), Times.Exactly(2));
        }

        [Theory]
        [InlineData("", "token")]
        [InlineData("userId", "")]
        [InlineData(null, "token")]
        [InlineData("userId", null)]
        public async Task ConfirmEmail_InvalidParameters_RedirectToHomeView(string userId, string token)
        {
            CreateController();

            var actionResult = await registerController.ConfirmEmail(userId, token);

            var viewResult = Assert.IsType<RedirectToActionResult>(actionResult);
            Assert.Equal("Home", viewResult.ControllerName);
            Assert.Equal("Index", viewResult.ActionName);
        }

        [Fact]
        public async Task ConfirmEmail_CannotFindUser_ReturnErrorViewWithErrors()
        {
            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult<ApplicationUser>(null));

            CreateController();

            var actionResult = await registerController.ConfirmEmail("testUserId", "testToken");

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.Equal(RegistrationStrings.EmailConfirmationError, viewResult.ViewData["ErrorMessage"]);
        }

        [Fact]
        public async Task ConfirmEmail_CannotConfirm_ReturnErrorViewWithErrors()
        {
            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new ApplicationUser()));
            _mockUserManager.Setup(um => um.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Failed()));

            CreateController();

            var actionResult = await registerController.ConfirmEmail("testUserId", "testToken");

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.Equal(RegistrationStrings.EmailConfirmationError, viewResult.ViewData["ErrorMessage"]);
        }

        [Fact]
        public async Task ConfirmEmail_Confirmed_RedirectToHomeView()
        {
            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new ApplicationUser()));
            _mockUserManager.Setup(um => um.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success));
           
            _mockManager.Setup(h => h.LogUserActionToDatabaseAsync(It.IsAny<ApplicationUser>(), It.IsAny<UserActionType>(), It.IsAny<string>())).Verifiable();

            CreateController();

            var actionResult = await registerController.ConfirmEmail("testUserId", "testToken");

            var viewResult = Assert.IsType<RedirectToActionResult>(actionResult);
            Assert.Equal("Home", viewResult.ControllerName);
            Assert.Equal("Index", viewResult.ActionName);
            _mockManager.Verify(h => h.LogUserActionToDatabaseAsync(It.IsAny<ApplicationUser>(), It.IsAny<UserActionType>(), It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public async Task IsUsernameInUse_NotInUse_ReturnJsonTrue()
        {
            _mockUserManager.Setup(um => um.FindByNameAsync(It.IsAny<string>()))
              .Returns(Task.FromResult<ApplicationUser>(null));

            CreateController();

            var actionResult = await registerController.IsUsernameInUse("test");

            var viewResult = Assert.IsType<JsonResult>(actionResult);
            Assert.True((bool)viewResult.Value);
        }

        [Fact]
        public async Task IsUsernameInUse_InUse_ReturnErrorMessageInJson()
        {
            _mockUserManager.Setup(um => um.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult<ApplicationUser>(new ApplicationUser()));

            CreateController();

            var actionResult = await registerController.IsUsernameInUse("test");

            var viewResult = Assert.IsType<JsonResult>(actionResult);
            Assert.Equal(string.Format(RegistrationStrings.UsernameIsTaken, "test"), (string)viewResult.Value);
        }

        [Fact]
        public async Task IsEmailInUse_NotInUse_ReturnJsonTrue()
        {
            _mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult<ApplicationUser>(null));

            CreateController();

            var actionResult = await registerController.IsEmailInUse("test");

            var viewResult = Assert.IsType<JsonResult>(actionResult);
            Assert.True((bool)viewResult.Value);
        }

        [Fact]
        public async Task IsEmailInUse_InUse_ReturnErrorMessageInJson()
        {
            _mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult<ApplicationUser>(new ApplicationUser()));

            CreateController();

            var actionResult = await registerController.IsEmailInUse("test");

            var viewResult = Assert.IsType<JsonResult>(actionResult);
            Assert.Equal(string.Format(RegistrationStrings.EmailIsTaken, "test"), (string)viewResult.Value);
        }
    }
}