using Janel.Data;
using PagedList.Core;

namespace Janel.Web.Models {
  public class PersonViewModel {
    public int PageSize { get; set; }
    public PagedList<Person> PersonList { get; set; }
  }
}
