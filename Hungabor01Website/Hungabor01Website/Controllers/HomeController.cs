using Hungabor01Website.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Hungabor01Website.Controllers
{
  /// <summary>
  /// Controller to handle views of the home, common pages
  /// </summary>
  public class HomeController : Controller
  {
    private readonly IServiceProvider serviceProvider;

    public HomeController(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Action to handle the home page
    /// </summary>
    /// <returns>The Index view</returns>
    public IActionResult Index()
    {
      using (var unitOfWork = serviceProvider.GetService<IUnitOfWork>())
      {
        var human = new Database.Entities.TestEntity { Name = "László" };
        unitOfWork.TestEntities.Add(human);
        unitOfWork.Complete();

        var entity = unitOfWork.TestEntities.Get(human.Id);
        ViewBag.Name = entity.Name;
      }
      return View();
    }

    /// <summary>
    /// Action to handle the privacy page
    /// </summary>
    /// <returns>The Privacy view</returns>
    public IActionResult Privacy()
    {      
      return View();
    }
  }
}
