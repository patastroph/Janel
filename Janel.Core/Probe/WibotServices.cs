using System.Collections.Generic;
using Janel.Contract;
using Janel.Core.Probe.Base;

namespace Janel.Core.Probe {
  public class WibotServices : ServicesProbe {
    public WibotServices(IAlertManager alertManager) : base(alertManager) {
    }

    public override List<string> Services => new List<string> { "RobotShopMSWiBotToWiBot", "RpcEptMapper" };
    public override string MachineName => "PLEGAULT-PC";
    public override string UserName => "test";
    public override string Password => "1qaZ2wsX";
    public override string Domain => "";
    public override int RetriesBeforeFailure => 3;

  }
}
