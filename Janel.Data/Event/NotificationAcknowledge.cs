using Observer.Core;

namespace Janel.Data.Event {
  public class NotificationAcknowledge : IEvent {
    public Notification Notification { get; }
    public Person By { get; }

    public NotificationAcknowledge(Notification notification, Person by) {
      Notification = notification;
      By = by;
    }
  }
}
