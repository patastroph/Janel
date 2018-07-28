using Janel.Contract;
using Janel.Core;
using Janel.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Janel.Web {
  public class Startup {
    public Startup(IConfiguration configuration) {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.AddMvc();

      services.AddScoped<IDateTimeManager, DateTimeManager>();
      services.AddScoped<IAlertManager, AlertManager>();
      services.AddScoped<INotificationManager, NotificationManager>();
      services.AddScoped<IPersonManager, PersonManager>();
      services.AddScoped<IScheduleManager, ScheduleManager>();
      services.AddScoped<IJanelUnitOfWork, JanelUnitOfWork>();

      services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

      services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, BackgroundTask>();

      JanelObserver.RegisterAllEvents(services.BuildServiceProvider());
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
      if (env.IsDevelopment()) {
        app.UseBrowserLink();
        app.UseDeveloperExceptionPage();
      } else {
        app.UseExceptionHandler("/Home/Error");
      }

      app.UseStaticFiles();

      app.UseMvc(routes => {
        routes.MapRoute(
            name: "default",
            template: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
