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
using Microsoft.AspNetCore.Identity;

namespace Janel.Web.Controllers {
  public class HomeController : Controller {
    private readonly IPersonManager _personManager;
    private readonly IAlertManager _alertManager;
    private readonly INotificationManager _notificationManager;
    private readonly IScheduleManager _scheduleManager;
    private UserManager<Person> _userManager;

    public HomeController(IPersonManager personManager, IAlertManager alertManager, INotificationManager notificationManager, IScheduleManager scheduleManager, UserManager<Person> userManager) {
      _personManager = personManager;
      _alertManager = alertManager;
      _notificationManager = notificationManager;
      _scheduleManager = scheduleManager;
      _userManager = userManager;
    }
    
    public IActionResult Index(int pageNumber = 1, int pageSize = 10) {
      var alertViewModel = _alertManager.GetOngoingAlerts()
                                          .Select(a => new AlertViewModel { Alert = a, PossibleActions = _alertManager.GetAlertPossibleActions(a) })
                                          .OrderBy(a => a.Alert.ReceivedAt);

      var currentSchedule = _scheduleManager.GetCurrentSchedule();
      var currentUserId = _userManager.GetUserId(User);
      var nextResponsible = _scheduleManager.GetNextPersonInCharge(currentSchedule?.Responsible);

      var viewModel = new AlertListViewModel {
        CurrentResponsibleIsLoggedUser = currentUserId == (currentSchedule?.Responsible?.Id?.ToString() ?? ""),
        CurrentResponsible = currentSchedule?.Responsible,
        CurrentResponsibleStatus = currentSchedule?.IsBusy,
        CurrentResponsibleStatusComment = currentSchedule?.BusyReason,
        CurrentResponsibleStartDate = currentSchedule?.StartAt,
        CurrentResponsibleEndDate = currentSchedule?.EndAt,
        Substitute = currentSchedule?.Substitute,
        NextResponsibleIsLoggedUser = currentUserId == (nextResponsible?.Id?.ToString() ?? ""),
        NextResponsible = nextResponsible,
        People = _personManager.GetPersonList().OrderBy(p => p.Name).ToList(),
        Alerts = new PagedList<AlertViewModel>(alertViewModel.AsQueryable(), pageNumber, pageSize)
      };

      return View(viewModel);
    }

    public IActionResult Call(Guid personId) {
      return View(_personManager.GetPerson(personId));
    }

    public IActionResult ConfirmCall(Guid personId, string message, CommunicationType communicationType) {
      var person = _personManager.GetPerson(personId);
      var sender = _personManager.GetPerson(User);

      message += $"\n\n- From : {sender.Name ?? sender.Email}";

      _notificationManager.SendNotification(person, message, communicationType);

      return RedirectToAction(nameof(Index));
    }

    public IActionResult ChangeAvailability() {
      var model = GetChangeAvailabilityViewModel();

      if (model == null) {
        return RedirectToAction(nameof(Index));
      }

      return View(model);
    }

    public IActionResult TakeItBack() {
      var currentSchedule = _scheduleManager.GetCurrentSchedule();
      _scheduleManager.SetPersonAsBack(currentSchedule.Responsible);

      return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult ConfirmChangeAvailability(string reason, Guid substitute) {
      try {
        var currentSchedule = _scheduleManager.GetCurrentSchedule();
        var currentUserId = _userManager.GetUserId(User);

        if (!string.IsNullOrEmpty(currentUserId) ||
            (!currentSchedule.Responsible.Id.Equals(new Guid(currentUserId)) && !(currentSchedule.Substitute?.Id).GetValueOrDefault().Equals(new Guid(currentUserId)))) {
          _scheduleManager.SetPersonAsBusy(currentSchedule.Responsible, reason, substitute);
        }
      }
      catch (Exception exc) {
        ModelState.AddModelError("", exc.Message);
        var model = GetChangeAvailabilityViewModel();
        return View(nameof(ChangeAvailability), model);
      }

      return Redirect("/");
    }

    public IActionResult Error() {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private ChangeAvailabilityViewModel GetChangeAvailabilityViewModel() {
      var currentSchedule = _scheduleManager.GetCurrentSchedule();
      var currentUserId = _userManager.GetUserId(User);

      if (string.IsNullOrEmpty(currentUserId) || 
          (!currentSchedule.Responsible.Id.Equals(new Guid(currentUserId)) && !(currentSchedule.Substitute?.Id).GetValueOrDefault().Equals(new Guid(currentUserId)))) {
        return null;
      }
      
      var model = new ChangeAvailabilityViewModel {
        IsAvailable = !currentSchedule.IsBusy,
        SwitchTo = currentSchedule.IsBusy ? "Available" : "Busy",
        LoggedUserIsResponsible = currentUserId == currentSchedule.Responsible.Id.ToString(),
        People = _personManager.GetPersonList().ToList(),
        Substitute = (_scheduleManager.GetNextPersonInCharge(currentSchedule.Responsible)?.Id).GetValueOrDefault()
      };

      return model;
    }
  }
}
