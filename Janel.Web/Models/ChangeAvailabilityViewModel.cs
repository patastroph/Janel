using System;
using System.Collections.Generic;
using Janel.Data;

namespace Janel.Web.Models
{
    public class ChangeAvailabilityViewModel
    {
      public string SwitchTo { get; set; }
      public bool IsAvailable { get; set; }
      public bool LoggedUserIsResponsible { get; set; }
      public IList<Person> People { get; set; }
      public Guid Substitute { get; set; }
    }
}
