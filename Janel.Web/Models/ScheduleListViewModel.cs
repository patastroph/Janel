using Janel.Data;
using PagedList.Core;
using System;
using System.Collections.Generic;

namespace Janel.Web.Models {
  public class ScheduleListViewModel {
    public PagedList<Schedule> Schedules { get; set; }
    public IList<Person> People { get; set; }
    public int Page { get; internal set; }
    public int PageSize { get; internal set; }
    public bool ShowPastSchedules { get; internal set; }
    public Guid? PersonId { get; internal set; }
  }
}
