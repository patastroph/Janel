using Janel.Contract.Repository;
using Janel.Data;
using System;
using System.Linq;

namespace Janel.Repository {
  public class ServiceRepository : BaseMongoDbRepository<Service, Service>, IServiceRepository {
    public override Service GetByName(string name) {
      return GetList().FirstOrDefault(l => l.Name.ToLower() == name.ToLower());
    }
  }
}
