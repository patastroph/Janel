using AutoMapper;
using Janel.Contract;
using Janel.Core;
using Janel.Data;
using Janel.Membership;
using Janel.Repository;
using Janel.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Observer.Core;

namespace Janel.Web {
  public class Startup {
    public Startup(IConfiguration configuration) {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.AddMvc(o => {
        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
        o.Filters.Add(new AuthorizeFilter(policy));
      });

      services.ConfigureIdentity();

      services.ConfigureIoC();

      JanelObserver.RegisterAllEvents(services.BuildServiceProvider());

      services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, BackgroundTask>();

      SetupAutoMapper();
    }

    private void SetupAutoMapper() {
      Mapper.Initialize(cfg => {
        cfg.CreateMap<Person, PersonEditViewModel>().ReverseMap();
      });
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

      app.UseAuthentication();

      app.UseMvc(routes => {
        routes.MapRoute(
            name: "default",
            template: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }

  public static class StartupExtensions {
    public static IServiceCollection ConfigureIdentity(this IServiceCollection services) {
      services.AddIdentity<Person, IdentityRole>(config => {
        config.User.RequireUniqueEmail = true;
      })
                .AddUserStore<UserStore>()
                .AddRoleStore<RoleStore>()
                .AddDefaultTokenProviders();

      return services;
    }


    public static IServiceCollection ConfigureIoC(this IServiceCollection services) {
      services.AddScoped<IDateTimeManager, DateTimeManager>();
      services.AddScoped<IAlertManager, AlertManager>();
      services.AddScoped<INotificationManager, NotificationManager>();
      services.AddScoped<IPersonManager, PersonManager>();
      services.AddScoped<IScheduleManager, ScheduleManager>();
      services.AddScoped<IJanelUnitOfWork, JanelUnitOfWork>();

      services.AddTransient<IEventListener, AlertManager>();
      services.AddTransient<IEventListener, Core.EventManager>();
      services.AddTransient<IEventListener, NotificationManager>();

      services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

      return services;
    }
  }
}
