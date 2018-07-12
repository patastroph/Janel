using Observer.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Janel.Data.Event {
  public class NotificationNotResponded : IEvent {
    public Notification Notification { get; }
    public NotificationNotResponded(Notification notification) {
      Notification = notification;
    }
  }
}
