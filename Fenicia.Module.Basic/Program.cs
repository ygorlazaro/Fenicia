using System.Text;

using Fenicia.Common;
using Fenicia.Common.API.Middlewares;
using Fenicia.Common.API.Providers;
using Fenicia.Common.Database.Contexts;
using Fenicia.Module.Basic.Domains.Customers;
using Fenicia.Module.Basic.Domains.State;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Scalar.AspNetCore;

namespace Fenicia.Module.Basic;

public class Program
{
    public static void Main(string[] args)
    {
        var tenantArg = args.FirstOrDefault(x => x.StartsWith("--tenant="));

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

        configBuilder.AddJsonFile(commonApiSettingsPath, optional: false, reloadOnChange: true);

        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddConfiguration(configBuilder);

        var key = Encoding.ASCII.GetBytes(configBuilder["Jwt:Secret"] ?? throw new InvalidOperationException(TextConstants.InvalidJwtSecretMessage));

        builder.Services.AddScoped<TenantProvider>();
        builder.Services.AddTransient<IStateService, StateService>();
        builder.Services.AddTransient<IStateRepository, StateRepository>();
        builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();

        builder.Services.AddDbContext<BasicContext>((sp, options) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var tenantProvider = sp.GetRequiredService<TenantProvider>();
            var tenantId = Environment.GetEnvironmentVariable("TENANT_ID") ?? tenantProvider.TenantId;
            var connString = config.GetConnectionString("Basic")?.Replace("{tenant}", tenantId);

            if (string.IsNullOrWhiteSpace(connString))
            {
                throw new Exception("Connection string inválida");
            }

            options.UseNpgsql(connString, b => b.MigrationsAssembly("Fenicia.Module.Basic")).EnableSensitiveDataLogging().UseSnakeCaseNamingConvention();
        });

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
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
            app.MapScalarApiReference(options =>
            {
                options.Authentication = new ScalarAuthenticationOptions
                {
                    PreferredSecuritySchemes = ["Bearer "]
                };
            });
        }

        app.UseAuthentication();
        app.UseMiddleware<TenantMiddleware>();
        app.UseAuthorization();

        app.UseWhen(context => context.Request.Path.StartsWithSegments("/basic"), appBuilder => appBuilder.UseModuleRequirement("basic"));

        app.MapControllers();

        app.Run();
    }
}
