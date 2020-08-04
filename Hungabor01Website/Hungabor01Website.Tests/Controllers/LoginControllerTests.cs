using BusinessLogic.ControllerManagers.Interfaces;
using Database.Core;
using Hungabor01Website.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Hungabor01Website.Tests.Controllers
{
    public class LoginControllerTests
    {
        private LoginController _loginController;

        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private readonly Mock<IAccountControllersManager> _mockManager;
        private readonly Mock<ILogger<LoginController>> _mockLogger;
        
        public LoginControllerTests()
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

            _mockLogger = new Mock<ILogger<LoginController>>();
        }

        private void CreateController()
        {
            _loginController = new LoginController(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockManager.Object,
                _mockLogger.Object);
        }

        [Fact]
        public void Index_CallAction_ReturnView()
        {

        }
    }
}
