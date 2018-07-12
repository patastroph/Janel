using Observer.Core;

namespace Janel.Data.Event {
  public class TaskTimerElapsed : IEvent {
    public int MinuteElapsed { get; }
    
    public TaskTimerElapsed(int minuteElapsed) {
      MinuteElapsed = minuteElapsed;
    }
  }
}
