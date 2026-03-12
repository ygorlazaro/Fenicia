using System.Text;

using Fenicia.Common.API.Middlewares;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Scalar.AspNetCore;

namespace Fenicia.Module.Plus;

public class Program
{
    public static void Main(string[] args)
    {
        var tenantArg = args.FirstOrDefault(o => o.StartsWith("--tenant="));
        if (tenantArg is not null)
        {
            var tenantId = tenantArg.Split("=")[1];

            Environment.SetEnvironmentVariable("TENANT_ID",
                tenantId);
        }

        var configBuilder = new ConfigurationManager();
        var commonApiSettingsPath =
            Path.Combine(Directory.GetCurrentDirectory(),
                "../Fenicia.Common.Api/appsettings.json");
        if (!File.Exists(commonApiSettingsPath))
        {
            throw new FileNotFoundException($"Could not find shared appsettings.json at {commonApiSettingsPath}");
        }

        configBuilder.AddJsonFile(commonApiSettingsPath,
            false,
            true);

        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddConfiguration(configBuilder);

        var key = Encoding.ASCII.GetBytes(configBuilder["Jwt:Secret"]
                                          ?? throw new InvalidOperationException("JWT secret key not found in configuration"));

        builder.Services.AddSingleton<ICompanyContext, CompanyContext>();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddDbContext<DefaultContext>((sp, o) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var connString = config.GetConnectionString("Auth");

            if (string.IsNullOrWhiteSpace(connString))
            {
                throw new Exception("Connection string inválida");
            }

            o.UseNpgsql(connString).EnableSensitiveDataLogging().UseSnakeCaseNamingConvention();
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
            app.MapScalarApiReference(x =>
            {
                x.Authentication = new ScalarAuthenticationOptions
                {
                    PreferredSecuritySchemes = ["Bearer "]
                };
            });
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseWhen(o => o.Request.Path.StartsWithSegments("/plus"),
            appBuilder => appBuilder.UseModuleRequirement("plus"));

        app.MapControllers();

        app.Run();
    }
}
