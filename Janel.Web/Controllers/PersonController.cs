using Janel.Contract;
using Janel.Data;
using Janel.Web.Models;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Janel.Web.Controllers {
  public class PersonController : Controller {
    private readonly IPersonManager _personManager;

    public PersonController(IPersonManager personManager) {
      _personManager = personManager;
    }

    public IActionResult Index(int page = 1, int pageSize = 10) {
      var list = _personManager.GetPersonList().OrderBy(p => p.Name);

      var viewModel = new PersonViewModel {
        Page = page,
        PageSize = pageSize,
        PersonList = new PagedList<Person>(list, page, pageSize)
      };

      return View(viewModel);
    }

    public IActionResult Edit(Guid id) {
      var model = _personManager.GetPerson(id);

      if (model.PreferedCommunications == null) {
        model.PreferedCommunications = new List<CommunicationType>();
      }
      var communications = Enum.GetValues(typeof(CommunicationType)).Cast<CommunicationType>();

      for (var i = model.PreferedCommunications.Count; i < communications.Count(); i++) {
        model.PreferedCommunications.Add(communications.ElementAt(i));
      }      

      return View(model);
    }

    public IActionResult Delete(Guid id) {
      _personManager.Delete(id);

      return RedirectToAction(nameof(Index));
    }

    public IActionResult Add() {
      return View(nameof(Edit));
    }

    [HttpPost]
    public IActionResult Save(Person person) {
      if (ModelState.IsValid) { 
        _personManager.Save(person);

        return RedirectToAction(nameof(Index));
      }
      
      return View(nameof(Edit), person);      
    }
  }
}