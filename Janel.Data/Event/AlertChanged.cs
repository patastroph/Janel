using Observer.Core;
using System.Collections.Generic;
using System.Linq;

namespace Janel.Data.Event {
  public class AlertChanged : IEvent {
    public Alert Alert { get; private set; }
    public List<string> Changes { get; private set; }

    public AlertChanged(Alert alert, params string[] changes) {
      Alert = alert;
      Changes = changes?.ToList() ?? new List<string>();
    }
  }
}
