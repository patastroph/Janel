using Janel.Contract.Repository;
using Janel.Data;
using MongoDB.Driver;
using System;
using System.Linq;

namespace Janel.Repository {
  public class ScheduleRepository : BaseMongoDbRepository<Schedule, Data.Schedule>, IScheduleRepository {
    public override Schedule GetByName(string name) {
      throw new NotImplementedException();
    }

    public override Data.Schedule BeforeSave(Schedule item) {
      var concrete = base.BeforeSave(item);
      concrete.ResponsibleId = item.Responsible?.Id;
      concrete.Responsible = null;
      concrete.SubstituteId = item.Substitute?.Id;
      concrete.Substitute = null;

      return concrete;
    }

    public override IQueryable<Schedule> GetList() {
      var query = from s in database.GetCollection<Data.Schedule>(CollectionName).AsQueryable()
                  join p in database.GetCollection<Person>(nameof(Person)).AsQueryable() on s.ResponsibleId equals p.Id // into ps
                  join sub in database.GetCollection<Person>(nameof(Person)).AsQueryable() on s.SubstituteId equals sub.Id into subs
                  //from p in ps.DefaultIfEmpty()
                  from sub in subs.DefaultIfEmpty()
                  select new Schedule {
                    Id = s.Id,
                    BusyReason = s.BusyReason,
                    EndAt = s.EndAt,
                    IsBusy = s.IsBusy,
                    StartAt = s.StartAt,
                    Responsible = p,
                    Substitute = sub
                  };

      return query;
    }
  }
}
