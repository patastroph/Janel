using Janel.Contract;
using Janel.Contract.Repository;

namespace Janel.Repository {
  public class JanelUnitOfWork : IJanelUnitOfWork {
    public IAlertRepository AlertRepository => new AlertRepository();

    public IPersonRepository PersonRepository => new PersonRepository();

    public IProbeRepository ProbeRepository => new ProbeRepository();

    public IServiceRepository ServiceRepository => new ServiceRepository();

    public INotificationRepository NotificationRepository => new NotificationRepository();

    public IScheduleRepository ScheduleRepository => new ScheduleRepository();

    public IRoleRepository RoleRepository => new RoleRepository();
  }
}
