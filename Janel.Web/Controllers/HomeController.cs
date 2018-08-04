using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Janel.Web.Models;
using Janel.Contract;
using System.Linq;
using Janel.Data;
using PagedList.Core;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;

namespace Janel.Web.Controllers {
  public class HomeController : Controller {
    private readonly IPersonManager _personManager;
    private readonly IAlertManager _alertManager;
    private readonly INotificationManager _notificationManager;

    public HomeController(IPersonManager personManager, IAlertManager alertManager, INotificationManager notificationManager) {
      _personManager = personManager;
      _alertManager = alertManager;
      _notificationManager = notificationManager;
    }
    
    public IActionResult Index(int pageNumber = 1, int pageSize = 10) {
      var alertViewModel = _alertManager.GetOngoingAlerts()
                                          .Select(a => new AlertViewModel { Alert = a, PossibleActions = _alertManager.GetAlertPossibleActions(a) })
                                          .OrderBy(a => a.Alert.ReceivedAt);

      var viewModel = new AlertListViewModel {
        People = _personManager.GetPersonList().OrderBy(p => p.Name).ToList(),
        Alerts = new PagedList<AlertViewModel>(alertViewModel.AsQueryable(), pageNumber, pageSize)
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

    public IActionResult Call(Guid personId) {
      return View(_personManager.GetPerson(personId));
    }

    public IActionResult ConfirmCall(Guid personId, string message, CommunicationType communicationType) {
      var person = _personManager.GetPerson(personId);

      _notificationManager.SendNotification(person, message, communicationType);

      return RedirectToAction(nameof(Index));
    }


    public IActionResult Error() {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
