using Janel.Data.Event;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Janel.Core {
  public class BackgroundTask : IHostedService {

    public async Task StartAsync(CancellationToken cancellationToken) {
      var loop = 0;
      var tasks = new List<int> { { 1 },
                                  { 3 },
                                  { 5 }};

      JanelObserver.EventManager.Dispatch(new AppStarting());

      while (!cancellationToken.IsCancellationRequested) {

        Parallel.ForEach(tasks, t => {
          if (loop % t == 0) {
            JanelObserver.EventManager.Dispatch(new TaskTimerElapsed(t));
          }
        });

        await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
        loop++;
      }
    }


    public Task StopAsync(CancellationToken cancellationToken) {
      JanelObserver.EventManager.Dispatch(new AppEnding());

      return Task.CompletedTask;
    }


  }
}
