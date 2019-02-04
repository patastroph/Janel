using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Janel.Contract;
using Janel.Data;
using Janel.Data.Event;
using Observer.Core;

namespace Janel.Core.Probe.Base {
  public abstract class WebProbe : IEventListener {
    private readonly IAlertManager _alertManager;
    public abstract string WebSite { get; }
    public abstract ProbeInterval ProbeEvery { get; }
    public abstract WebProbeType ProbeType { get; }
    public abstract string PageContent { get; }
    public abstract int TimeOutInSeconds { get; }
    public abstract int RetriesBeforeFailure { get; }
    public int Retries = 0;
    public bool Enabled = true;

    public WebProbe(IAlertManager alertManager) {
      _alertManager = alertManager;
    }

    public void RegisterEvents(IEventManager eventManager) {
      eventManager.Register<TaskTimerElapsed>().To(Probe).When(t => t.MinuteElapsed == (int)ProbeEvery);
    }

    private IEnumerable<Message> Probe(TaskTimerElapsed arg) {
      try
      {
        var failureMessage = "Alert !\n";

        using (var client = new HttpClient()) {
          client.Timeout = TimeSpan.FromSeconds(TimeOutInSeconds);
          client.DefaultRequestHeaders.Add("x-probing", "janel");

          using (var request = client.GetAsync(WebSite).Result) {
            switch (ProbeType) {
              case WebProbeType.HttpCode200:

                if (request.StatusCode != HttpStatusCode.OK) {
                  Retries++;
                }
                else {
                  Retries = 0;
                }
                break;
              case WebProbeType.ContentInPage:
                var content = request.Content.ReadAsStringAsync().Result;

                if (!content.Contains(PageContent)) {
                  Retries++;
                }
                else {
                  Retries = 0;
                }

                break;
            }
          }
        }

        if (Enabled && Retries >= RetriesBeforeFailure) {
          _alertManager.LogAlert(failureMessage, WebSite, Enum.GetName(typeof(WebProbeType), ProbeType), "", SeverityType.Moderate);
        }
      }
      catch (Exception e) {
        _alertManager.LogAlert($"Unable to probe {WebSite}. {e.Message}", WebSite, Enum.GetName(typeof(WebProbeType), ProbeType), "", SeverityType.Moderate);
      }

      return JanelObserver.Success();
    }
  }

  public enum WebProbeType {
    HttpCode200,
    ContentInPage
  }
}
