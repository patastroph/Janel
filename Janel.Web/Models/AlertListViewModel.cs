using Janel.Data;
using PagedList.Core;
using System.Collections.Generic;

namespace Janel.Web.Models {
  public class AlertListViewModel {
    public IDictionary<int, string> PossibleActions { get; set; }
    public IList<Person> People { get; set; }
    public IPagedList<AlertViewModel> Alerts { get; set; }
  }
}
