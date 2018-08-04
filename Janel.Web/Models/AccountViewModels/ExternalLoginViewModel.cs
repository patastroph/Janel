using System.ComponentModel.DataAnnotations;

namespace Janel.Web.Models.AccountViewModels {
  public class ExternalLoginViewModel {
    [Required]
    [EmailAddress]
    public string Email { get; set; }
  }
}
