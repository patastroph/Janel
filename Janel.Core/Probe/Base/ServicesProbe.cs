using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using Janel.Contract;
using Janel.Data;
using Janel.Data.Event;
using Observer.Core;

namespace Janel.Core.Probe.Base {
  public abstract class ServicesProbe : IEventListener {
    private readonly IAlertManager _alertManager;
    public abstract List<string> Services { get; }
    public abstract string MachineName { get; }
    public abstract string UserName { get; }
    public abstract string Password { get; }
    public abstract string Domain { get; }
    public abstract int RetriesBeforeFailure { get; }
    public int Retries = 0;
    public bool Enabled = true;

    public ServicesProbe(IAlertManager alertManager) {
      _alertManager = alertManager;
    }

    public void RegisterEvents(IEventManager eventManager) {
      eventManager.Register<TaskTimerElapsed>().To(Probe).When(t => t.MinuteElapsed == (int)ProbeInterval.FiveMinutes);
    }

    private IEnumerable<Message> Probe(TaskTimerElapsed arg) {
      try {
        using (var authentication = new WindowsLogin(UserName, Domain, Password)) {
          var failedServices = new List<string>();

          WindowsIdentity.RunImpersonated(authentication.Identity.AccessToken, () => {

            foreach (var service in Services) {
              var windowsService = new ServiceController(service, MachineName);

              if (windowsService.Status != ServiceControllerStatus.Running) {
                failedServices.Add(service);
              }
            }

          });

          if (Enabled && failedServices.Any() && Retries++ >= RetriesBeforeFailure) {
            _alertManager.LogAlert($"Alert !\n\nServices not running : \n{string.Join("\n- ", failedServices)}", "Windows Services", MachineName, "", SeverityType.Critical);
          }
          else if (!Enabled || !failedServices.Any()) {
            Retries = 0;
          }
        }
      }
      catch (Exception exc) {
        _alertManager.LogAlert($"Unable to probe services of {MachineName}. {exc.Message}", MachineName, "", "", SeverityType.Moderate);
      }

      return JanelObserver.Success();
    }
  }
}
