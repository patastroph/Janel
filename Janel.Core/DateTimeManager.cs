using Janel.Contract;
using System;

namespace Janel.Core {
  public class DateTimeManager : IDateTimeManager {
    public DateTime GetNow() {
      return DateTime.Now;
    }
  }
}
