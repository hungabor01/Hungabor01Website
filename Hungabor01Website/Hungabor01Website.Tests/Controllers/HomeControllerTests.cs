using Hungabor01Website.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Hungabor01Website.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly HomeController _homeController;

        public HomeControllerTests()
        {
            _homeController = new HomeController();
        }

        [Fact]
        public void Index_CallAction_ReturnView()
        {
            var actionResult = _homeController.Index();

            var viewResult = Assert.IsType<ViewResult>(actionResult);

            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void Privacy_CallAction_ReturnView()
        {
            var actionResult = _homeController.Privacy();

            var viewResult = Assert.IsType<ViewResult>(actionResult);

            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void Contacts_CallAction_ReturnView()
        {
            var actionResult = _homeController.Contacts();

            var viewResult = Assert.IsType<ViewResult>(actionResult);

            Assert.Null(viewResult.ViewName);
        }
    }
}
