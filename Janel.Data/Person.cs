using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Janel.Data {
  public class Person : Entity {
    public Person() {
      PreferedCommunications = new List<CommunicationType>();
    }

    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public int Level { get; set; }
    public List<CommunicationType> PreferedCommunications { get; set; }
    public string Password { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
  }
}
