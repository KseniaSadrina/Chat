using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Chat.Hubs;
using Microsoft.Extensions.Configuration;
using Chat.DAL;
using Microsoft.EntityFrameworkCore;
using Models;
using Microsoft.AspNetCore.Identity;
using Chat.Helpers;
using Microsoft.Extensions.Logging;

namespace Chat
{
	public class Startup
	{
    public Startup(IHostingEnvironment env)
    {
      Configuration = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).AddJsonFile("appSettings.json").Build();
    }

    public IConfigurationRoot Configuration { get; private set; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
		{
      services.AddDbContext<ChatContext>
          (options => options.UseSqlite(Configuration["ConnectionStrings:DefaultConnection"]));
      ConfigureAuthentication(services);
      services.AddScoped<ITrainingService, TrainingService>();
      services.AddScoped<ISessionsService, SessionService>();
      services.AddMvc();
      services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
      {
        builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithOrigins("http://localhost:4200");
      }));
      // SignalR and CORS
      services.AddSignalR();
    }

    private void ConfigureAuthentication(IServiceCollection services)
    {

      services.AddIdentity<User, Role>()
          .AddEntityFrameworkStores<ChatContext>()
          .AddDefaultTokenProviders();

      services.AddAuthentication().AddGoogle(googleOptions =>
      {
        googleOptions.ClientId = Configuration.GetSection("Authentication").GetSection("Google")["ClientId"];
        googleOptions.ClientSecret = Configuration.GetSection("Authentication").GetSection("Google")["ClientSecret"];
      });

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public async void Configure(IApplicationBuilder app, IHostingEnvironment env, UserManager<User> userManager, RoleManager<Role> roleManager, ILogger<Startup> logger)
		{
			app.Use(async (context, next) => {
				await next();
				if (context.Response.StatusCode == 404 &&
				   !Path.HasExtension(context.Request.Path.Value) &&
				   !context.Request.Path.Value.StartsWith("/api/"))
				{
					context.Request.Path = "/index.html";
					await next();
				}
			});

      await roleManager.SeedRoles(logger);
      await userManager.SeedUserRoles(logger);

      app.UseCors("MyPolicy");
      app.UseWebSockets();
      app.UseSignalR(routes =>
      {
        routes.MapHub<MessageHub>("/message");
        routes.MapHub<TrainingHub>("/training");
      });
      app.UseMvcWithDefaultRoute();
			app.UseDefaultFiles();
			app.UseStaticFiles();
      app.UseAuthentication();
    }
  }
}
