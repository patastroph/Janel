using Observer.Core;
using System;

namespace Janel.Data.Event {
  public class ErrorOccurred : IEvent {
    public Exception Exception { get; }
    public string Error { get; }

    public ErrorOccurred(Exception exception) {
      Exception = exception;
    }

    public ErrorOccurred(string error) {
      Error = error;
    }
  }
}
