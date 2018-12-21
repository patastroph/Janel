using System;
using System.ComponentModel.DataAnnotations;

namespace Janel.Data {
  public class Schedule : Entity {
    [Required]
    public Person Responsible { get; set; }
    [Required]
    public DateTime StartAt { get; set; }
    [Required]
    public DateTime EndAt { get; set; }
    public bool IsBusy { get; set; }
    public string BusyReason { get; set; }
    public Person Substitute { get; set; }
  }
}
