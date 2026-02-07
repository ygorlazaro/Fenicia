using System.Text;

using Fenicia.Common;
using Fenicia.Common.API.Middlewares;
using Fenicia.Common.API.Providers;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.SocialNetwork.Domains.Feed;
using Fenicia.Module.SocialNetwork.Domains.Follower;
using Fenicia.Module.SocialNetwork.Domains.User;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Scalar.AspNetCore;

namespace Fenicia.Module.SocialNetwork;

public class Program
{
    public static void Main(string[] args)
    {
        var tenantArg = args.FirstOrDefault(o => o.StartsWith("--tenant="));
        if (tenantArg is not null)
        {
            var tenantId = tenantArg.Split("=")[1];

            Environment.SetEnvironmentVariable("TENANT_ID", tenantId);
        }

        var configBuilder = new ConfigurationManager();
        var commonApiSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "../Fenicia.Common.Api/appsettings.json");
        if (!File.Exists(commonApiSettingsPath))
        {
            throw new FileNotFoundException($"Could not find shared appsettings.json at {commonApiSettingsPath}");
        }

        configBuilder.AddJsonFile(commonApiSettingsPath, false, true);

        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddConfiguration(configBuilder);

        var key = Encoding.ASCII.GetBytes(configBuilder["Jwt:Secret"] ?? throw new InvalidOperationException(TextConstants.InvalidJwtSecretMessage));

        builder.Services.AddScoped<TenantProvider>();

        builder.Services.AddTransient<IUserRepository, UserRepository>();
        builder.Services.AddTransient<IUserService, UserService>();
        builder.Services.AddTransient<IFeedRepository, FeedRepository>();
        builder.Services.AddTransient<IFeedService, FeedService>();
        builder.Services.AddTransient<IFollowerRepository, FollowerRepository>();
        builder.Services.AddTransient<IFollowerService, FollowerService>();

        builder.Services.AddDbContext<SocialNetworkContext>((sp, o) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var tenantProvider = sp.GetRequiredService<TenantProvider>();

            var tenantId = Environment.GetEnvironmentVariable("TENANT_ID") ?? tenantProvider.TenantId;

            var connString = config.GetConnectionString("SocialNetwork")?.Replace("{tenant}", tenantId);

            if (string.IsNullOrWhiteSpace(connString))
            {
                throw new Exception("Connection string inválida");
            }

            o.UseNpgsql(connString, b => b.MigrationsAssembly("Fenicia.Module.SocialNetwork")).EnableSensitiveDataLogging().UseSnakeCaseNamingConvention();
        });

        builder.Services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.RequireHttpsMetadata = false;
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidIssuer = "AuthService",
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };
        });

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(o =>
            {
                o.Authentication = new ScalarAuthenticationOptions
                {
                    PreferredSecuritySchemes = ["Bearer "]
                };
            });
        }

        app.UseAuthentication();
        app.UseMiddleware<TenantMiddleware>();
        app.UseAuthorization();

        app.UseWhen(o => o.Request.Path.StartsWithSegments("/socialnetwork"), appBuilder => appBuilder.UseModuleRequirement("socialnetwork"));

        app.MapControllers();

        app.Run();
    }
}
