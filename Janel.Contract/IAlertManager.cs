using Janel.Data;
using System;
using System.Collections.Generic;

namespace Janel.Contract {
  public interface IAlertManager {
    void LogAlert(string description, string serviceName, string serviceInfo, string serviceIp, SeverityType severity = SeverityType.Unknown);
    List<Alert> GetOngoingAlerts();
    List<Alert> GetPastAlerts();
    List<string> GetAlertPossibleActions(Alert alert);
    void Acknowledge(Alert alert, Person responsible);
    void CannotHandle(Alert alert, Person responsible, ReasonType reason);
    void Fixed(Alert alert, Person responsible);
    void Snooze(Alert alert, Person responsible, int minuteSnoozed);
    void Complete(Alert alert, Person responsible);   
  }
}
