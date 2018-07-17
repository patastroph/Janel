using System;

namespace Janel.Repository.Data {
  public class Schedule : Janel.Data.Schedule {
    public Guid? ResponsibleId { get; set; }    
  }
}
