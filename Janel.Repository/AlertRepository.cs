using Janel.Contract.Repository;
using Janel.Data;

namespace Janel.Repository {
  public class AlertRepository : BaseMongoDbRepository<Alert>, IAlertRepository {
    public override Alert GetByName(string name) {
      throw new System.NotImplementedException();
    }
  }
}
