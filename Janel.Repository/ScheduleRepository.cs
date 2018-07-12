using Janel.Contract.Repository;
using Janel.Data;
using System;

namespace Janel.Repository {
  public class ScheduleRepository : BaseMongoDbRepository<Schedule>, IScheduleRepository {
    public override Schedule GetByName(string name) {
      throw new NotImplementedException();
    }
  }
}
