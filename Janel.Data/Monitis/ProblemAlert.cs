using System;
using System.Collections.Generic;
using System.Text;

namespace Janel.Data.Monitis
{
    public class ProblemAlert
    {
      public int alertId { get; set; }
      public string time { get; set; }
      public string name { get; set; }
      public string alertType { get; set; }
      public string failedLocations { get; set; }
      public string adddata { get; set; }
      public string group { get; set; }
      public string type { get; set; }
      public string url { get; set; }
  }
}
