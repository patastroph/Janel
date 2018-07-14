using Janel.Contract;
using Janel.Data;
using System;
using System.Linq;

namespace Janel.Core {
  public class PersonManager : IPersonManager {
    private readonly IJanelUnitOfWork _unitOfWork;

    public PersonManager(IJanelUnitOfWork unitOfWork) {
      _unitOfWork = unitOfWork;
    }

    public void Delete(Guid id) {
      _unitOfWork.PersonRepository.Delete(id);
    }

    public void Delete(Person person) {
      _unitOfWork.PersonRepository.Delete(person);
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
