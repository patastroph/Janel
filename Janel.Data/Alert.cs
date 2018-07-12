using System;
using System.Collections.Generic;

namespace Janel.Data {
  public class Alert : Entity {
    public string Description { get; set; }
    public SeverityType Severity { get; set; }
    public StatusType Status { get; set; }
    public Service Service { get; set; }
    public Person Responsible { get; set; }
    public List<Person> Actors { get; set; }
    public DateTime ReceivedAt { get; set; }
    public DateTime CompletedAt { get; set; }
    public int NbReceived { get; set; }
    public List<DateTime> NotificationsSent { get; set; }
    public DateTime UpdatedAt { get; set; }
  }
}
