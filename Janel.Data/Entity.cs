using System;
using System.ComponentModel.DataAnnotations;

namespace Janel.Data {
  public abstract class Entity {
    [Key]
    public Guid? Id { get; set; }
  }
}
