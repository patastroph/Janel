using Janel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Janel.Web.Models {
  public class PersonEditViewModel {
    public Guid? Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Phone]
    public string PhoneNumber { get; set; }
    public int Level { get; set; }
    public List<CommunicationType> PreferedCommunications { get; set; }
  }
}
