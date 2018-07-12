using System;

namespace Janel.Data {
  public class Communication {
    public Person SentTo { get; set; }
    public DateTime SentOn { get; set; }
    public CommunicationType Type { get; set; }
    public bool IsSentSuccessfully { get; set; }
    public bool IsAcknowledge { get; set; }
  }
}
