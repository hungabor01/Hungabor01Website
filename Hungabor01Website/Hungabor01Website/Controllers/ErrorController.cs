using Hungabor01Website.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hungabor01Website.Controllers
{
  /// <summary>
  /// Controller for error handling
  /// </summary>
  [Route("Error")]
  public class ErrorController : Controller
  {
    private readonly ILogger<ErrorController> logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
      this.logger = logger;
    }

    /// <summary>
    /// Action to handle errors with status code
    /// </summary>
    /// <param name="errorCode">Status code of the error, coöming from the browser</param>
    /// <returns>The Error view</returns>
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
      logger.LogWarning(EventIds.ExceptionCodeHandlerError, errorMessage);
      return View("Error");
    }
    
    /// <summary>
    /// Action to handle the general unhandled errors in the code
    /// </summary>
    /// <returns>The Error view</returns>
    [AllowAnonymous]
    [Route("UnhandledError")]
    public IActionResult UnhandledError()
    {
      var exceptionHandlerPathFeature =
        HttpContext.Features.Get<IExceptionHandlerPathFeature>();
      var errorMessage = exceptionHandlerPathFeature.Error.Message;
      ViewBag.ErrorMessage = errorMessage;
      logger.LogWarning(EventIds.UnhandledErrorError, errorMessage);
      return View("Error");
    }
  }
}