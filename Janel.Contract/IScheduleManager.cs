using Janel.Data;
using System;

namespace Janel.Contract {
  public interface IScheduleManager {
    void SetPersonAsBusy(Person responsible, string reason);
    void SetPersonAsBack(Person responsible);

    Person GetPersonInCharge();
    Person GetNextPersonInCharge(Person nextFromPerson);
    Person GetEscalatePerson();

    void AddSchedule(Person responsible, DateTime startDate, DateTime endDate);
    void RemoveSchedule(Person responsible, DateTime startDate, DateTime endDate);
    void RemoveSchedule(Schedule schedule);
  }
}