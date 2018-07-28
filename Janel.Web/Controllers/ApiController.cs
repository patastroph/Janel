using Janel.Contract;
using Janel.Data;
using Microsoft.AspNetCore.Mvc;

namespace Janel.Web.Controllers {
  public class ApiController : ControllerBase {
    private readonly IAlertManager _alertManager;

    public ApiController(IAlertManager alertManager) {
      _alertManager = alertManager;
    }

    public ActionResult RegisterAlert(string description, string serviceName, string serviceInfo, string serviceIp, SeverityType severity = SeverityType.Unknown) {
      _alertManager.LogAlert(description, serviceInfo, serviceInfo, serviceIp, severity);

      return Ok();
    }
  }
}
