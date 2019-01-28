using Janel.Contract;
using Janel.Data;
using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Janel.Core {
  public class PersonManager : IPersonManager {
    private readonly IJanelUnitOfWork _unitOfWork;
    private readonly UserManager<Person> _userManager;

    public PersonManager(IJanelUnitOfWork unitOfWork, UserManager<Person> userManager)
    {
      _unitOfWork = unitOfWork;
      _userManager = userManager;
    }

    public void Delete(Guid id) {
      _unitOfWork.PersonRepository.Delete(id);
    }

    public void Delete(Person person) {
      _unitOfWork.PersonRepository.Delete(person);
    }

    public Person GetPerson(ClaimsPrincipal user)
    {
      var userId = _userManager.GetUserId(user);

      if (Guid.TryParse(userId, out var gUserId)){
        return _unitOfWork.PersonRepository.GetById(gUserId);
      }

      return null;
    }

    public Person GetPerson(Guid id) {
      return _unitOfWork.PersonRepository.GetById(id);
    }

    public IQueryable<Person> GetPersonList() {
      return _unitOfWork.PersonRepository.GetList();
    }

    public void Save(Person person) {
      if (!person.Id.HasValue) {
        //Validate Unicity ...
        _unitOfWork.PersonRepository.Insert(person);
      } else {
        _unitOfWork.PersonRepository.Update(person);
      }
    }
  }
}
