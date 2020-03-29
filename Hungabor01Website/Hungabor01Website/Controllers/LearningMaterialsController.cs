using Microsoft.AspNetCore.Mvc;

namespace Hungabor01Website.Controllers
{
    public class LearningMaterialsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}