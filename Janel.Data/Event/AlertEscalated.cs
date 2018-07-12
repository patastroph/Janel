using Observer.Core;

namespace Janel.Data.Event {
  public class AlertEscalated : IEvent {
    public Alert Alert;
    public Person EscalatePerson;

    public AlertEscalated(Alert alert, Person escalatePerson) {
      Alert = alert;
      EscalatePerson = escalatePerson;
    }
  }
}
