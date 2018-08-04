using Janel.Contract.Repository;

namespace Janel.Contract {
  public interface IJanelUnitOfWork {
    IAlertRepository AlertRepository { get; }
    IPersonRepository PersonRepository { get; }
    IRoleRepository RoleRepository { get; }
    IProbeRepository ProbeRepository { get; }
    IServiceRepository ServiceRepository { get; }  
    INotificationRepository NotificationRepository { get; }
    IScheduleRepository ScheduleRepository { get; }
  }
}
