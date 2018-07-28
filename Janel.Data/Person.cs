using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Janel.Data {
  public class Person : Entity {
    public Person() {
      PreferedCommunications = new List<CommunicationType>();
    }

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
