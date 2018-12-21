using Observer.Core;

namespace Janel.Data.Event {
  public class MessageWithAcknowledge : IEvent {
    public MessageWithAcknowledge(Person from, Person to, string message) {
      From = from;
      To = to;
      Message = message;
    }

    public Person From { get; set; }
    public Person To { get; set; }
    public string Message { get; set; }
  }

}
