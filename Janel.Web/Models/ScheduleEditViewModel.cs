using Janel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Janel.Web.Models {
  public class ScheduleEditViewModel {
    public IList<Person> People { get; set; }
    [Key]
    public Guid? Id { get; set; }
    [Required]
    public DateTime? StartAt { get; set; }
    [Required]
    public DateTime? EndAt { get; set; }
    [Required]
    public Guid? ResponsibleId { get; set; }
  }
}
