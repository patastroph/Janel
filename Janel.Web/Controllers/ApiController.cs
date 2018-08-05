using Janel.Contract;
using Janel.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Janel.Web.Controllers {
  public class ApiController : ControllerBase {
    private readonly IAlertManager _alertManager;

    public ApiController(IAlertManager alertManager) {
      _alertManager = alertManager;
    }

    [AllowAnonymous]
    public ActionResult RegisterAlert(string description, string serviceName, string serviceInfo, string serviceIp, SeverityType severity = SeverityType.Unknown) {
      //http://localhost:55378/Api/RegisterAlert?description=Test%20Alert&serviceName=Serv1&serviceInfo=Magento&serviceIP=127.0.0.1
      _alertManager.LogAlert(description, serviceName, serviceInfo, serviceIp, severity);

      return Ok();
    }
  }
}
