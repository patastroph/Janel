using System.Collections.Generic;
using Janel.Contract;
using Janel.Data;
using Janel.Data.Event;
using Observer.Core;

namespace Janel.Core.Probe
{
    public class Monitis : IEventListener
    {
      private readonly IAlertManager _alertManager;

      public Monitis(IAlertManager alertManager) {
        _alertManager = alertManager;
      }

      public void RegisterEvents(IEventManager eventManager) {
        eventManager.Register<TaskTimerElapsed>().To(Probe).When(t => t.MinuteElapsed == (int)ProbeInterval.FiveMinutes);
      }

      private IEnumerable<Message> Probe(TaskTimerElapsed arg) {
        _alertManager.LogAlert("Timeout from Monitis", "www.robotshop.com", "EC2BLABLA", "127.0.0.1", SeverityType.Critical);

        return JanelObserver.Success();
      }
    }
}
