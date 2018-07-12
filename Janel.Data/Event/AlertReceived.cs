using Observer.Core;

namespace Janel.Data.Event {
  public class AlertReceived :IEvent {
    public Alert Alert { get; }
    public Person OverridePerson { get; }

    public AlertReceived(Alert alert, Person overridePerson = null) {
      Alert = alert;
      OverridePerson = overridePerson;
    }        
  }
}
