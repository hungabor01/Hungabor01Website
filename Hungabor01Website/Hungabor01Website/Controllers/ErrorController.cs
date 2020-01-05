using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Hungabor01Website.Controllers
{
  [Route("Error")]
  public class ErrorController : Controller
  {
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