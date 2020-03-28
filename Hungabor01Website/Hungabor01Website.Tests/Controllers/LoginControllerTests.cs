using Hungabor01Website.Controllers;
using Hungabor01Website.DataAccess.Managers.Interfaces;
using Hungabor01Website.Database.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Hungabor01Website.Tests.Controllers
{
    public class LoginControllerTests
    {
        private LoginController loginController;

        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private readonly Mock<ILogger<LoginController>> _mockLogger;
        private readonly Mock<ILoginManager> _mockManager;

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

            _mockLogger = new Mock<ILogger<LoginController>>();
            _mockManager = new Mock<ILoginManager>();
        }

        private void CreateController()
        {
            loginController = new LoginController(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockLogger.Object,
                _mockManager.Object);
        }

        [Fact]
        public void Index_CallAction_ReturnView()
        {

        }
    }
}
