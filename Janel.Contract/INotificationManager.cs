using Janel.Data;
using System;

namespace Janel.Contract {
  public interface INotificationManager {
    void AcknowledgeNotification(Guid notificationId, Person by);
    bool SendNotification(Person to, string message, CommunicationType type);
  }
}
