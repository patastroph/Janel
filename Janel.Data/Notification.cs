using System;
using System.Collections.Generic;

namespace Janel.Data {
  public class Notification : Entity {
    public string Message { get; set; }
    public bool IsAcknowledge { get; set; }
    public object Source { get; set; }
    public List<Communication> MessagesSent { get; set; }    
    public DateTime AcknowledgeOn { get; set; }
    public Person AcknowledgeBy { get; set; }

    public Notification() {
      MessagesSent = new List<Communication>();
    }
  }
}
