using Janel.Contract;
using Janel.Core.Probe.Base;

namespace Janel.Core.Probe.Web
{
    public class Signifyd : WebProbe
    {
      public Signifyd(IAlertManager alertManager) : base(alertManager)
      { }

      public override string WebSite => "https://www.signifyd.com/";
      public override ProbeInterval ProbeEvery => ProbeInterval.OneMinute;
      public override WebProbeType ProbeType => WebProbeType.ContentInPage;
      public override string PageContent => "Orders";
      public override int TimeOutInSeconds => 30;
      public override int RetriesBeforeFailure => 3;
    }
}
