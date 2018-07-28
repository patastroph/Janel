using Janel.Data;
using System;
using System.Linq;

namespace Janel.Contract {
  public interface IPersonManager {
    Person GetPerson(Guid id);
    IQueryable<Person> GetPersonList();
    void Save(Person person);
    void Delete(Guid id);
    void Delete(Person person);    
  }
}
