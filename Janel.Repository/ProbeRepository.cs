using Janel.Contract.Repository;
using Janel.Data;
using System;
using System.Linq;

namespace Janel.Repository
{
  public class ProbeRepository : BaseMongoDbRepository<Probe>, IProbeRepository {
    public override Probe GetByName(string name) {
      return GetList().FirstOrDefault(l => l.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
  }
}
