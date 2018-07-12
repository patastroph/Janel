using Janel.Contract;
using Janel.Web.Models;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;
using System;

namespace Janel.Web.Controllers {
  public class PersonController : Controller {
    private readonly IPersonManager _personManager;

    public PersonController(IPersonManager personManager) {
      _personManager = personManager;
    }

    public IActionResult Index(int page = 1, int pageSize = 10) {
      var list = _personManager.GetPersonList();

      var viewModel = new PersonViewModel {
        PageSize = pageSize,
        PersonList = new PagedList<Data.Person>(list, page, pageSize)
      };

      return View(viewModel);
    }

    public IActionResult Edit(Guid id) {
      var viewModel = new PersonEditViewModel {
        Person = _personManager.GetPerson(id)
      };

      return View(viewModel);
    }

    public IActionResult Add() {
      return View("Edit");
    }

    [HttpPost]
    public IActionResult Save(PersonEditViewModel editViewModel) {
      _personManager.Save(editViewModel.Person);

      return RedirectToAction("Index");
    }
  }
}