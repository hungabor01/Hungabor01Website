using Hungabor01Website.Constants;
using Hungabor01Website.Constants.Strings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hungabor01Website.Controllers
{
    [Route("Error")]
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        [Route("ErrorCodeHandler/{errorCode}")]
        public IActionResult ExceptionCodeHandler(int errorCode)
        {
            ViewBag.ErrorCode = errorCode;
            var errorMessage = errorCode switch
            {
                404 => Strings.Error404,
                _ => Strings.UnexpectedError,
            };
            ViewBag.ErrorMessage = errorMessage;
            _logger.LogWarning(EventIds.ExceptionCodeHandlerError, errorMessage);
            return View("Error");
        }
    
        [AllowAnonymous]
        [Route("UnhandledError")]
        public IActionResult UnhandledError()
        {
            string errorMessage;
            try
            {
                var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
                errorMessage = exceptionHandlerPathFeature.Error.Message;
            }
            catch
            {
                errorMessage = Strings.UnexpectedError;
            }

            ViewBag.ErrorMessage = errorMessage;
            _logger.LogWarning(EventIds.UnhandledErrorError, errorMessage);
            return View("Error");
        }
    }
}