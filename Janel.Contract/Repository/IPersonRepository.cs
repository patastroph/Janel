using Janel.Data;

namespace Janel.Contract.Repository {
  public interface IPersonRepository : IBaseRepository<Person> {
    Person GetByUserName(string normalizedUserName);
  }
}
