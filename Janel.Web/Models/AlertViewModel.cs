using Janel.Data;
using System.Collections.Generic;

namespace Janel.Web.Models {
  public class AlertViewModel {
    public Alert Alert { get; set; }
    public List<string> PossibleActions { get; set; }
    public string SelectedAction { get; set; }
  }
}
