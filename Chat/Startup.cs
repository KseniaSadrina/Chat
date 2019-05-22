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
using Chat.Marley;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Chat.Services;
using System.Threading.Tasks;

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
      services.AddOptions();
      services.Configure<NlpConfiguration>(Configuration.GetSection(nameof(NlpConfiguration)));
      services.Configure<TokenConfiguration>(Configuration.GetSection(nameof(TokenConfiguration)));

      services.AddDbContext<ChatContext>
          (options => options.UseSqlite(Configuration["ConnectionStrings:DefaultConnection"]));

      // Gerneral Services
      services.AddScoped<IAuthenticationService, JWTAuthenticationService>();
      services.AddScoped<IGoalsService, GoalsService>();

      // NLP
      services.AddScoped<INlpService, BertService>();
      services.AddScoped<IBotService, MarleyService>();

      // DAL
      services.AddScoped<ITrainingService, TrainingService>();
      services.AddScoped<ISessionsService, SessionService>();
      services.AddScoped<IGoalsService, GoalsService>();

      services.AddMvc();
      ConfigureAuthentication(services);
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
      var tokenConfig = Configuration.GetSection(nameof(TokenConfiguration)).Get<TokenConfiguration>();
      var key = Encoding.ASCII.GetBytes(tokenConfig?.Secret);

      services.AddIdentity<User, Role>()
          .AddEntityFrameworkStores<ChatContext>() 
          .AddDefaultTokenProviders();

      services.AddAuthentication(x =>
      {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      }).AddJwtBearer(x =>
      {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenConfig.Secret)),
          ValidIssuer = tokenConfig.Issuer,
          ValidAudience = tokenConfig.Audience,
          ValidateIssuer = false,
          ValidateAudience = false
        };
        x.Events = new JwtBearerEvents
        {
          OnMessageReceived = context =>
          {
            var accessToken = context.Request.Query["access_token"];
            // If the request is for our hub...
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                ((path.StartsWithSegments("/message") || (path.StartsWithSegments("/training")))))
            {
              // Read the token out of the query string
              context.Token = accessToken;
            }
            return Task.CompletedTask;
          },
          OnAuthenticationFailed = ex =>
          {
            return Task.CompletedTask;
          },
        };
      });
      services.AddAuthorization(options =>
      {
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public async void Configure(IApplicationBuilder app,
      IHostingEnvironment env,
      UserManager<User> userManager,
      RoleManager<Role> roleManager,
      ILogger<Startup> logger,
      ChatContext dbContext)
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

      app.UseAuthentication();
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
    }
  }
}
