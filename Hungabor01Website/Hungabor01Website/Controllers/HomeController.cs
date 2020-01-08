using Hungabor01Website.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Hungabor01Website.Controllers
{
  public class HomeController : Controller
  {
    private IServiceProvider serviceProvider;

    public HomeController(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    public IActionResult Index()
    {
      using (var unitOfWork = serviceProvider.GetService<IUnitOfWork>())
      {
        unitOfWork.TestEntities.Add(new Database.Entities.TestEntity { Name="László"});
        unitOfWork.Complete();

        var entity = unitOfWork.TestEntities.Get(2);
        ViewBag.Name = entity.Name;
      }
      return View();
    }

    public IActionResult Privacy()
    {      
      return View();
    }
  }
}
