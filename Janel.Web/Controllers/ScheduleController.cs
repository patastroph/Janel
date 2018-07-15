using System;
using System.Linq;
using Janel.Contract;
using Janel.Data;
using Janel.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;

namespace Janel.Web.Controllers {
  public class ScheduleController : Controller {
    private readonly IScheduleManager _scheduleManager;
    private readonly IPersonManager _personManager;

    public ScheduleController(IScheduleManager scheduleManager, IPersonManager personManager) {
      _scheduleManager = scheduleManager;
      _personManager = personManager;
    }

    // GET: Schedule
    public ActionResult Index(int pageNumber = 1, int pageSize = 10, bool showPastSchedules = false, Guid? personId = null) {
      var schedules = _scheduleManager.GetAll(showPastSchedules, personId).OrderBy(s => s.StartAt).ThenBy(s => s.EndAt);

      var viewModel = new ScheduleListViewModel {
        Page = pageNumber,
        PageSize = pageSize,
        ShowPastSchedules = showPastSchedules,
        PersonId = personId,
        People = _personManager.GetAll().OrderBy(p => p.Name).ToList(),
        Schedules = new PagedList<Schedule>(schedules, pageNumber, pageSize)
      };

      return View(viewModel);
    }
    
    // GET: Schedule/Add
    public ActionResult Add() {
      var viewModel = new ScheduleEditViewModel {
        People = _personManager.GetAll().OrderBy(p => p.Name).ToList()
      };

      return View(nameof(Edit), viewModel);
    }

    // POST: Schedule/Save
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Save(ScheduleEditViewModel viewModel) {
      try {
        if (ModelState.IsValid) {

          if (viewModel.Id.HasValue) {
            _scheduleManager.EditSchedule(viewModel.Id.Value, viewModel.ResponsibleId.Value, viewModel.StartAt.Value, viewModel.EndAt.Value);
          } else {
            _scheduleManager.AddSchedule(viewModel.ResponsibleId.Value, viewModel.StartAt.Value, viewModel.EndAt.Value);
          }          

          return RedirectToAction(nameof(Index));
        }

        viewModel.People = _personManager.GetAll().OrderBy(p => p.Name).ToList();

        return View(nameof(Edit), viewModel);
      } catch (Exception exc) {
        ModelState.AddModelError("", exc.Message);

        viewModel.People = _personManager.GetAll().OrderBy(p => p.Name).ToList();

        return View(nameof(Edit), viewModel);
      }
    }

    // GET: Schedule/Edit/{Guid}
    public ActionResult Edit(Guid id) {
      var schedule = _scheduleManager.GetSchedule(id);

      var viewModel = new ScheduleEditViewModel {
        Id = schedule?.Id,
        StartAt = schedule?.StartAt,
        EndAt = schedule?.EndAt,
        ResponsibleId = schedule?.Responsible?.Id,
        People = _personManager.GetAll().OrderBy(p => p.Name).ToList()
      };

      return View(viewModel);
    }

    // POST: Schedule/Delete/5     
    public ActionResult Delete(Guid id) {
      try {
        _scheduleManager.RemoveSchedule(id);

        return RedirectToAction(nameof(Index));
      } catch {
        return View();
      }
    }
  }
}