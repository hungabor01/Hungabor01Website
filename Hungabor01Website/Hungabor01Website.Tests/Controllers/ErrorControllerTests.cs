using Hungabor01Website.Constants.Strings;
using Hungabor01Website.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace Hungabor01Website.Tests.Controllers
{
    public class ErrorControllerTests
    {
        private readonly ErrorController _errorController;
        private readonly Mock<ILogger<ErrorController>> _mockLogger;

        public ErrorControllerTests()
        {
            _mockLogger = new Mock<ILogger<ErrorController>>();
            _errorController = new ErrorController(_mockLogger.Object);
        }

        [Fact]
        public void ExceptionCodeHandler_StatusCode404_ReturnViewWithErrorMessage()
        {
            var actionResult = _errorController.ExceptionCodeHandler(404);

            var viewResult = Assert.IsType<ViewResult>(actionResult);

            Assert.Equal("Error", viewResult.ViewName);
            Assert.Equal(2, viewResult.ViewData.Count);
            Assert.Equal(404, viewResult.ViewData["ErrorCode"]);
            Assert.Equal(Strings.Error404, viewResult.ViewData["ErrorMessage"]);

            _mockLogger.Verify(l => l.Log(
              It.IsAny<LogLevel>(),
              It.IsAny<EventId>(),
              It.IsAny<It.IsAnyType>(),
              It.IsAny<Exception>(),
              (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
              Times.Once);
        }

        [Fact]
        public void ExceptionCodeHandler_NoStatusCode_ReturnViewWithErrorMessage()
        {
            var actionResult = _errorController.ExceptionCodeHandler(0);

            var viewResult = Assert.IsType<ViewResult>(actionResult);

            Assert.Equal("Error", viewResult.ViewName);
            Assert.Equal(2, viewResult.ViewData.Count);
            Assert.Equal(0, viewResult.ViewData["ErrorCode"]);
            Assert.Equal(Strings.UnexpectedError, viewResult.ViewData["ErrorMessage"]);

            _mockLogger.Verify(l => l.Log(
              It.IsAny<LogLevel>(),
              It.IsAny<EventId>(),
              It.IsAny<It.IsAnyType>(),
              It.IsAny<Exception>(),
              (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
              Times.Once);
        }

        [Fact]
        public void UnhandledError_NoErrorInHttpContext_ReturnViewWithErrorMessage()
        {
            var actionResult = _errorController.UnhandledError();

            var viewResult = Assert.IsType<ViewResult>(actionResult);

            Assert.Equal("Error", viewResult.ViewName);
            Assert.Single(viewResult.ViewData);
            Assert.Equal(Strings.UnexpectedError, viewResult.ViewData["ErrorMessage"]);

            _mockLogger.Verify(l => l.Log(
              It.IsAny<LogLevel>(),
              It.IsAny<EventId>(),
              It.IsAny<It.IsAnyType>(),
              It.IsAny<Exception>(),
              (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
              Times.Once);
        }
    }
}