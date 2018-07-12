using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Observer.Core;
using Observer.SqlEventHandler;

namespace Janel.Core {
  public class JanelObserver {
    private static Observer.Core.EventManager _eventManager;
    private static SqlEventHandler _eventErrorHandler;

    public static IEventErrorHandler EventErrorHandler => _eventErrorHandler ?? (_eventErrorHandler = new SqlEventHandler());
    public static IEventManager EventManager => _eventManager ?? (_eventManager = new Observer.Core.EventManager(EventErrorHandler));

    public static void RegisterAllEvents(ServiceProvider serviceProvider) {
      foreach (var listener in serviceProvider.GetServices<IEventListener>()) {
        listener.RegisterEvents(EventManager);
      }
    }

    internal static IEnumerable<Message> Success() {
      return new List<Message> { new Message { Succeeded = true } };
    }
  }
}
