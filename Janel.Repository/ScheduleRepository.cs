using Janel.Contract.Repository;
using Janel.Data;
using System;

namespace Janel.Repository {
  public class ScheduleRepository : BaseMongoDbRepository<Schedule>, IScheduleRepository {
    public override Schedule GetByName(string name) {
      throw new NotImplementedException();
    }

    public override Schedule Update(Schedule item) {
      SetDate(item);

      return base.Update(item);
    }

    public override Schedule Insert(Schedule item) {
      SetDate(item);

      return base.Insert(item);
    }

    private void SetDate(Schedule schedule) {
      schedule.StartAt = schedule.StartAt.ToUniversalTime();
      schedule.EndAt = schedule.EndAt.ToUniversalTime();
    }
  }
}
