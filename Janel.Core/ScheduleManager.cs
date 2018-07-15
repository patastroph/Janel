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
      var conflicts = HasConflicts(startDate, endDate);

      if (conflicts.Any()) {
        throw new Exception("There is a conflict with this schedule. Please validate Start Date and End Date");
      }
      
      var schedule = new Schedule {
        Responsible = responsible,
        StartAt = startDate,
        EndAt = endDate
      };

      ValidateSchedule(schedule);

      _unitOfWork.ScheduleRepository.Insert(schedule);
    }

    

    public void AddSchedule(Guid responsibleId, DateTime startDate, DateTime endDate) {
      var responsible = _unitOfWork.PersonRepository.GetById(responsibleId);

      AddSchedule(responsible, startDate, endDate);
    }

    public void EditSchedule(Guid scheduleId, Guid responsibleId, DateTime startAt, DateTime endAt) {
      var schedule = _unitOfWork.ScheduleRepository.GetById(scheduleId);
      var responsible = _unitOfWork.PersonRepository.GetById(responsibleId);

      schedule.Responsible = responsible;
      schedule.StartAt = startAt;
      schedule.EndAt = endAt;

      EditSchedule(schedule);
    }

    public void EditSchedule(Schedule schedule) {
      var conflicts = HasConflicts(schedule.StartAt, schedule.EndAt);

      if (conflicts.Where(s => !s.Id.Equals(schedule.Id)).Any()) {
        throw new Exception("There is a conflict with this schedule. Please validate Start Date and End Date");
      }

      ValidateSchedule(schedule);

      _unitOfWork.ScheduleRepository.Update(schedule);
    }
    
    public IQueryable<Schedule> GetAll(bool showPastSchedules, Guid? personId) {
      if (personId.HasValue) {
        return _unitOfWork.ScheduleRepository.GetList().Where(s => (showPastSchedules || s.EndAt >= _dateTimeManager.GetNow()) && s.Responsible.Id.Equals(personId.Value));
      }

      return _unitOfWork.ScheduleRepository.GetList().Where(s => (showPastSchedules || s.EndAt >= _dateTimeManager.GetNow()));
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

    public Schedule GetSchedule(Guid id) {
      return _unitOfWork.ScheduleRepository.GetById(id);
    }

    public void RemoveSchedule(Person responsible, DateTime startDate, DateTime endDate) {
      var schedule = _unitOfWork.ScheduleRepository.GetList().Where(s => (s.StartAt == null || s.StartAt <= startDate) && (s.EndAt == null || s.EndAt >= endDate) && s.Responsible.Id.Equals(responsible.Id)).FirstOrDefault();

      if (schedule == null) {
        throw new Exception("No schedule founded to remove");
      }

      RemoveSchedule(schedule);
    }

    public void RemoveSchedule(Schedule schedule) {
      RemoveSchedule(schedule.Id.Value);
    }

    public void RemoveSchedule(Guid scheduleId) {
      _unitOfWork.ScheduleRepository.Delete(scheduleId);
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

    private void ValidateSchedule(Schedule schedule) {
      if (schedule.EndAt <= schedule.StartAt) {
        throw new ArgumentException("Start At must be before End At");
      }
    }

    private IQueryable<Schedule> HasConflicts(DateTime startDate, DateTime endDate) {
      return _unitOfWork.ScheduleRepository.GetList().Where(s => ((s.StartAt == null || s.StartAt <= startDate) && (s.EndAt == null || s.EndAt >= endDate)) ||
                                                                 ((s.StartAt == null || startDate <= s.StartAt) && (s.EndAt == null || endDate >= s.EndAt)));
    }
  }
}
