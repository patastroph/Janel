using Janel.Data;
using System;
using System.Linq;

namespace Janel.Contract {
  public interface IScheduleManager {
    void SetPersonAsBusy(Person responsible, string reason);
    void SetPersonAsBack(Person responsible);

    Person GetPersonInCharge();
    Person GetNextPersonInCharge(Person nextFromPerson);
    Person GetEscalatePerson();

    void AddSchedule(Person responsible, DateTime startDate, DateTime endDate);
    void AddSchedule(Guid responsibleId, DateTime startDate, DateTime endDate);
    void EditSchedule(Guid scheduleId, Guid responsibleId, DateTime startAt, DateTime endAt);
    void EditSchedule(Schedule schedule);
    void RemoveSchedule(Person responsible, DateTime startDate, DateTime endDate);
    void RemoveSchedule(Schedule schedule);
    void RemoveSchedule(Guid scheduleId);
    IQueryable<Schedule> GetAll(bool showPastSchedules, Guid? personId);
    Schedule GetSchedule(Guid id);
    Schedule GetCurrentSchedule();
    
  }
}