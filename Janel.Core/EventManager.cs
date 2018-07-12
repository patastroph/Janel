using Janel.Data.Event;
using Observer.Core;
using System;
using System.Collections.Generic;

namespace Janel.Core {
  public class EventManager : IEventListener {
    public void RegisterEvents(IEventManager eventManager) {
      JanelObserver.EventManager.Register<AlertChanged>().To(LogAlertChanged);
    }

    private IEnumerable<Message> LogAlertChanged(AlertChanged alert) {
      throw new NotImplementedException();
    }
  }
}
