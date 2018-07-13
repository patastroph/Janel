using Janel.Contract;
using Janel.Data;
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
      return View(_personManager.GetPerson(id));
    }

    public IActionResult Delete(Guid id) {
      _personManager.Delete(id);

      return RedirectToAction("Index");
    }

    public IActionResult Add() {
      return View("Edit");
    }

    [HttpPost]
    public IActionResult Save(Person person) {
      //if (ModelState.IsValid) { Need fix to work with Id
        _personManager.Save(person);
      //}

      return RedirectToAction("Index");
    }
  }
}