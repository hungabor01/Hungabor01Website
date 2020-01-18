using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hungabor01Website.Controllers
{
  /// <summary>
  /// Controller to handle views of the home, common pages
  /// </summary>
  public class HomeController : Controller
  {
    /// <summary>
    /// Action to handle the home page
    /// </summary>
    /// <returns>The Index view</returns>
    [AllowAnonymous]
    public IActionResult Index()
    {
      return View();
    }

    /// <summary>
    /// Action to handle the privacy page
    /// </summary>
    /// <returns>The Privacy view</returns>
    [AllowAnonymous]
    public IActionResult Privacy()
    {
      return View();
    }

    /// <summary>
    /// Action to handle the contacts page
    /// </summary>
    /// <returns>The Contacts view</returns>
    [AllowAnonymous]
    public IActionResult Contacts()
    {
      return View();
    }
  }
}
