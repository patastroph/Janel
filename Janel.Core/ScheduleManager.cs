using Janel.Contract;
using Janel.Data;
using System;
using System.Linq;

namespace Janel.Core {
  public class ScheduleManager : IScheduleManager {
    private readonly IJanelUnitOfWork _unitOfWork;
    private readonly IDateTimeManager _dateTimeManager;

    public ScheduleManager(IJanelUnitOfWork unitOfWork, IDateTimeManager dateTimeManager) {
      _unitOfWork = unitOfWork;
      _dateTimeManager = dateTimeManager;
    }

    public void AddSchedule(Person responsible, DateTime startDate, DateTime endDate) {
      var conflicts = _unitOfWork.ScheduleRepository.GetList().Where(s => (s.StartAt == null || s.StartAt <= startDate) && (s.EndAt == null || s.EndAt >= endDate));

      if (conflicts.Any()) {
        throw new Exception("There is a conflict with this schedule. Please validate Start Date and End Date");
      }

      var schedule = new Schedule {
        Responsible = responsible,
        StartAt = startDate,
        EndAt = endDate
      };

      _unitOfWork.ScheduleRepository.Insert(schedule);
    }

    public Person GetEscalatePerson() {
      return _unitOfWork.PersonRepository.GetList().Where(p => p.Level >= 3).OrderByDescending(p => p.Level).FirstOrDefault();
    }
    
    public Person GetNextPersonInCharge(Person nextFromPerson) {
      var now = DateTime.Now;
      var schedules = _unitOfWork.ScheduleRepository.GetList().Where(s => (s.StartAt <= now && s.EndAt >= now && !s.IsBusy) || s.StartAt > now).OrderBy(s => s.StartAt);

      if (!schedules.Any()) {
        throw new Exception("No schedule founded for next person in charge");
      }

      if (schedules.Any(s => s.Responsible.Id.Equals(nextFromPerson.Id))) {
        //At least, the responsible has been found (so he is not busy at the moment)
        var responsibleFounded = false;

        foreach (var schedule in schedules) {
          if (schedule.Responsible.Id.Equals(nextFromPerson.Id)) {
            responsibleFounded = true;
            continue;
          }

          if (responsibleFounded && !schedule.IsBusy) {
            return schedule.Responsible;
          }
        }

        return null; //No one founded
      }

      return schedules.FirstOrDefault(s => !s.IsBusy)?.Responsible;
    }

    public Person GetPersonInCharge() {
      var now = _dateTimeManager.GetNow();
      var schedule = _unitOfWork.ScheduleRepository.GetList().Where(s => (s.StartAt <= now && s.EndAt >= now && !s.IsBusy) || s.StartAt > now).OrderBy(s => s.StartAt).FirstOrDefault();
       
      if (schedule == null) {
        throw new Exception("There is no responsible found");
      }

      return schedule.Responsible;
    }

    public void RemoveSchedule(Person responsible, DateTime startDate, DateTime endDate) {
      var schedule = _unitOfWork.ScheduleRepository.GetList().Where(s => (s.StartAt == null || s.StartAt <= startDate) && (s.EndAt == null || s.EndAt >= endDate) && s.Responsible.Id.Equals(responsible.Id)).FirstOrDefault();

      if (schedule == null) {
        throw new Exception("No schedule founded to remove");
      }

      RemoveSchedule(schedule);
    }

    public void RemoveSchedule(Schedule schedule) {
      _unitOfWork.ScheduleRepository.Delete(schedule.Id);
    }

    public void SetPersonAsBack(Person responsible) {
      var now = DateTime.Now;
      var schedule = _unitOfWork.ScheduleRepository.GetList().Where(s => ((s.StartAt <= now && s.EndAt >= now) || s.StartAt > now) && 
                                                                          s.Responsible.Id.Equals(responsible.Id) && s.IsBusy).OrderBy(s => s.StartAt).FirstOrDefault();

      if (schedule != null) {
        schedule.IsBusy = false;        
        _unitOfWork.ScheduleRepository.Update(schedule);
      } else {
        throw new Exception($"Buzy schedule not found for {responsible.Name}");
      }
    }

    public void SetPersonAsBusy(Person responsible, string reason) {
      var now = DateTime.Now;
      var schedule = _unitOfWork.ScheduleRepository.GetList().Where(s => ((s.StartAt <= now && s.EndAt >= now) || s.StartAt > now) && s.Responsible.Id.Equals(responsible.Id)).OrderBy(s => s.StartAt).FirstOrDefault();

      if (schedule != null) {
        schedule.IsBusy = true;
        schedule.BusyReason = reason;
        _unitOfWork.ScheduleRepository.Update(schedule);
      } else {
        throw new Exception($"Active schedule not found for {responsible.Name}");
      }
    }
  }
}
