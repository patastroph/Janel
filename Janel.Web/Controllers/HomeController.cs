using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Janel.Web.Models;
using Janel.Contract;
using System.Linq;
using Janel.Data;
using PagedList.Core;
using System.Collections.Generic;

namespace Janel.Web.Controllers {
  public class HomeController : Controller {
    private readonly IPersonManager _personManager;
    private readonly IAlertManager _alertManager;

    public HomeController(IPersonManager personManager, IAlertManager alertManager) {
      _personManager = personManager;
      _alertManager = alertManager;
    }

    public IActionResult Index(int pageNumber = 1, int pageSize = 10) {
      var viewModel = new AlertListViewModel {
        People = _personManager.GetPersonList().OrderBy(p => p.Name).ToList(),
        Alerts = new PagedList<Alert>(_alertManager.GetOngoingAlerts().OrderBy(a => a.ReceivedAt).AsQueryable(), pageNumber, pageSize),
        //PossibleActions = new Dictionary<int, string>()
      };

      return View(viewModel);
    }

    public IActionResult About() {
      ViewData["Message"] = "Your application description page.";

      return View();
    }

    public IActionResult Contact() {
      ViewData["Message"] = "Your contact page.";

      return View();
    }

    public IActionResult Error() {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
