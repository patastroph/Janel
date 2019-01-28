using Janel.Data;
using System;
using System.Linq;
using System.Security.Claims;

namespace Janel.Contract {
  public interface IPersonManager {
    Person GetPerson(Guid id);
    IQueryable<Person> GetPersonList();
    void Save(Person person);
    void Delete(Guid id);
    void Delete(Person person);
    Person GetPerson(ClaimsPrincipal user);
  }
}
