using Janel.Contract.Repository;
using Janel.Data;

namespace Janel.Repository
{
  public class NotificationRepository : BaseMongoDbRepository<Notification>, INotificationRepository {
    public override Notification GetByName(string name) {
      throw new System.NotImplementedException();
    }
  }
}
