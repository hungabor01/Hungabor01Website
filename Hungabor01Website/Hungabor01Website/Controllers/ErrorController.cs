using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Hungabor01Website.Controllers
{
  /// <summary>
  /// Controller for error handling
  /// </summary>
  [Route("Error")]
  public class ErrorController : Controller
  {
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
      ViewBag.ErrorMessage = errorCode switch
      {
        404 => "The resource you requested could not be found.",
        _ => "An unexpected error has occured.",
      };
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
      var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

      ViewBag.ErrorMessage = exceptionHandlerPathFeature.Error.Message;

      return View("Error");
    }
  }
}