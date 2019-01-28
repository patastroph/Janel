using Janel.Data;
using PagedList.Core;
using System;
using System.Collections.Generic;

namespace Janel.Web.Models {
  public class AlertListViewModel {
    public IDictionary<int, string> PossibleActions { get; set; }
    public IList<Person> People { get; set; }
    public IPagedList<AlertViewModel> Alerts { get; set; }
    public bool CurrentResponsibleIsLoggedUser { get; internal set; }
    public Person CurrentResponsible { get; internal set; }
    public bool? CurrentResponsibleStatus { get; internal set; }
    public string CurrentResponsibleStatusComment { get; internal set; }
    public DateTime? CurrentResponsibleStartDate { get; internal set; }
    public DateTime? CurrentResponsibleEndDate { get; internal set; }
    public Person NextResponsible { get; internal set; }
    public bool NextResponsibleIsLoggedUser { get; internal set; }
    public Person Substitute { get; set; }
  }
}
