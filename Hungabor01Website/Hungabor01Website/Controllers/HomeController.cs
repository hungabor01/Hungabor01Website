using Microsoft.AspNetCore.Mvc;

namespace Hungabor01Website.Controllers
{
  public class HomeController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Privacy()
    {      
      return View();
    }
  }
}
