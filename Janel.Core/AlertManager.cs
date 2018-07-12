using Janel.Contract;
using Janel.Data;
using Janel.Data.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using Observer.Core;

namespace Janel.Core {
  public class AlertManager : IAlertManager, IEventListener {
    private static List<Alert> _ongoingAlerts = new List<Alert>();
    private static List<Alert> _pastAlerts = new List<Alert>();
    private readonly IScheduleManager _scheduleManager;
    private readonly INotificationManager _notificationManager;

    private IJanelUnitOfWork _unitOfWork { get; }

    public AlertManager(IJanelUnitOfWork unitOfWork, IScheduleManager scheduleManager, INotificationManager notificationManager) {
      _unitOfWork = unitOfWork;
      _scheduleManager = scheduleManager;
      _notificationManager = notificationManager;
    }

    public void RegisterEvents(IEventManager eventManager) {
      eventManager.Register<NotificationNotResponded>().To(OnAlertNotResponded).When(n => n.Notification.Source is Alert);
      eventManager.Register<NotificationAcknowledge>().To(Acknowledge).When(n => n.Notification.Source is Alert);
      eventManager.Register<TaskTimerElapsed>().To(ValidatePendingAlerts).When(t => t.MinuteElapsed == 5);
    }
    
    public List<Alert> GetOngoingAlerts() {
      return _ongoingAlerts;
    }

    public List<Alert> GetPastAlerts() {
      return _pastAlerts;
    }

    private IEnumerable<Message> ValidatePendingAlerts(TaskTimerElapsed arg) {
      foreach (var alert in _ongoingAlerts) {
        switch (alert.Status) {
          case StatusType.Acknowledge:
          case StatusType.Fixed:
          case StatusType.Transferring:          
            if (alert.UpdatedAt.AddMinutes(60) >= DateTime.Now) {
              EscalateAlert(alert);
            }
            break;
          case StatusType.Escalated:
            if (alert.UpdatedAt.AddMinutes(60) >= DateTime.Now) {
              //In Deep shit !
              var newResponsible = _scheduleManager.GetNextPersonInCharge(alert.Responsible);

              alert.Status = StatusType.Transferring;
              alert.Description += $"\n\nAlert were escalated but was still not responded.";

              _unitOfWork.AlertRepository.Update(alert);

              JanelObserver.EventManager.Dispatch(new AlertChanged(alert, StatusType.Transferring.ToString()));
              JanelObserver.EventManager.Dispatch(new AlertReceived(alert, newResponsible));
            }
            break;
        }

      }

      return JanelObserver.Success();
    }

    private IEnumerable<Message> Acknowledge(NotificationAcknowledge arg) {
      Acknowledge((Alert)arg.Notification.Source, arg.By);

      return JanelObserver.Success();
    }

    private IEnumerable<Message> OnAlertNotResponded(NotificationNotResponded arg) {
      var alert = arg.Notification.Source as Alert;
      
      if (alert.Status == StatusType.Transferring) {
        //Tranfert failed. Message back to responsible
        var firstResponsible = alert.Actors.First();

        _notificationManager.SendNotification(firstResponsible, $"Alert cannot be transfered. {alert.Responsible.Name} did not respond", firstResponsible.PreferedCommunications.First());

        alert.Responsible = firstResponsible;
        alert.Status = StatusType.New;

        _unitOfWork.AlertRepository.Update(alert);
      } else {
        EscalateAlert(alert);
      }

      return JanelObserver.Success();
    }

    public void EscalateAlert(Alert alert) {
      if (alert == null || !_ongoingAlerts.Any(a => a.Id.Equals(alert.Id))) {
        throw new Exception("Alert not found");
      }

      var escalatePerson = _scheduleManager.GetEscalatePerson();

      if (escalatePerson == null) {
        JanelObserver.EventManager.Dispatch(new ErrorOccurred($"No one responded to alert {alert.Id}"));        
      } else {
        alert.Status = StatusType.Escalated;

        _unitOfWork.AlertRepository.Update(alert);
        JanelObserver.EventManager.Dispatch(new AlertEscalated(alert, escalatePerson));
      }      
    }

    public void LogAlert(string description, string serviceName, string serviceInfo, string serviceIp, SeverityType severity = SeverityType.Unknown) {
      Alert alert;
      if ((alert = _ongoingAlerts.FirstOrDefault(a => a.Service.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase) && a.Description.Equals(description, StringComparison.OrdinalIgnoreCase))) == null) {

        alert = new Alert {
          Severity = CalculateSeverity(alert),
          ReceivedAt = DateTime.Now,
          Description = description,
          Service = _unitOfWork.ServiceRepository.GetByName(serviceName) ?? _unitOfWork.ServiceRepository.Insert(new Service { Name = serviceName, Location = serviceInfo, Ip = serviceIp }),
          NbReceived = 1
        };

        _ongoingAlerts.Add(alert);

        _unitOfWork.AlertRepository.Insert(alert);

        JanelObserver.EventManager.Dispatch(new AlertReceived(alert));
      } else {
        alert.NbReceived++;
        _unitOfWork.AlertRepository.Update(alert);
      }
    }

    public void Acknowledge(Alert alert, Person responsible) {
      if (alert == null || !_ongoingAlerts.Any(a => a.Id.Equals(alert.Id))) {
        throw new Exception("Alert not found");
      }

      alert.Responsible = responsible;
      alert.Status = StatusType.Acknowledge;

      _unitOfWork.AlertRepository.Update(alert);
      JanelObserver.EventManager.Dispatch(new AlertChanged(alert, StatusType.Acknowledge.ToString(), responsible.Name));
    }

    public void Fixed(Alert alert, Person responsible) {
      if (alert == null || !_ongoingAlerts.Any(a => a.Id.Equals(alert.Id))) {
        throw new Exception("Alert not found");
      }

      alert.CompletedAt = DateTime.Now;
      alert.Status = StatusType.Fixed;

      _unitOfWork.AlertRepository.Update(alert);
      JanelObserver.EventManager.Dispatch(new AlertChanged(alert, StatusType.Fixed.ToString(), responsible.Name));
    }

    public void Snooze(Alert alert, Person responsible, int minuteSnoozed) {
      throw new NotImplementedException();
    }

    public void Complete(Alert alert, Person responsible) {
      if (alert == null || !_ongoingAlerts.Any(a => a.Id.Equals(alert.Id))) {
        throw new Exception("Alert not found");
      }

      alert.CompletedAt = DateTime.Now;
      alert.Status = StatusType.Closed;

      _unitOfWork.AlertRepository.Update(alert);

      _ongoingAlerts.Remove(alert);
      _pastAlerts.Add(alert);

      JanelObserver.EventManager.Dispatch(new AlertChanged(alert, StatusType.Closed.ToString(), responsible.Name));
    }

    public void CannotHandle(Alert alert, Person responsible, ReasonType reason) {
      var nextReponsible = _scheduleManager.GetNextPersonInCharge(responsible);

      if (nextReponsible == null) {
        JanelObserver.EventManager.Dispatch(new ErrorOccurred($"No one can handle alert {alert.Id}"));
        return;
      }

      alert.Status = StatusType.Transferring;
      alert.Description += $"\n\n {responsible.Name} cannot take it because {reason.ToString()}";

      _unitOfWork.AlertRepository.Update(alert);

      JanelObserver.EventManager.Dispatch(new AlertChanged(alert, StatusType.Transferring.ToString(), responsible.Name));
      JanelObserver.EventManager.Dispatch(new AlertReceived(alert, nextReponsible));
    }

    private SeverityType CalculateSeverity(Alert alert) {
      if (alert.Severity != SeverityType.Unknown) {
        return alert.Severity;
      }

      // Need to be calculated
      return SeverityType.Moderate;
    }


  }
}
