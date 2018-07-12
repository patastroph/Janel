using System;

namespace Janel.Data {
  public class Schedule : Entity {
    public Person Responsible { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public bool IsBusy { get; set; }
    public string BusyReason { get; set; }
  }
}
