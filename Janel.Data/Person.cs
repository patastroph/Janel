using System.Collections.Generic;

namespace Janel.Data {
  public class Person : Entity {
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public int Level { get; set; }
    public List<CommunicationType> PreferedCommunications { get; set; }
  }
}
