using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ttt
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
          options.AccessDeniedPath = "/api/user/ValidateAsync";
          options.LoginPath = "/api/user/ValidateAsync";
        });

      services.AddControllers();

      services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });
    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
        app.UseDeveloperExceptionPage();
      else
        app.UseExceptionHandler("/Error");

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseSpaStaticFiles();

      app.UseRouting();
      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller}/{action=Index}/{id?}");
      });

      app.UseSpa(spa =>
      {
        spa.Options.SourcePath = "ClientApp";

        if (env.IsDevelopment())
          spa.UseReactDevelopmentServer(npmScript: "start");
      });
    }
  }
}
