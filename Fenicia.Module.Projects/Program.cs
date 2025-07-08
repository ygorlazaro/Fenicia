namespace Fenicia.Module.Projects;

using System.Text;

using Common;
using Common.Api.Middlewares;
using Common.Api.Providers;

using Contexts;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Scalar.AspNetCore;

public class Program
{
    public static void Main(string[] args)
    {
        var tenantArg = args.FirstOrDefault(x => x.StartsWith(value: "--tenant="));
        if (tenantArg is not null)
        {
            var tenantId = tenantArg.Split(separator: "=")[1];

            Environment.SetEnvironmentVariable(variable: "TENANT_ID", tenantId);
        }

        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        var key = Encoding.ASCII.GetBytes(configuration[key: "Jwt:Secret"] ?? throw new InvalidOperationException(TextConstants.InvalidJwtSecret));

        builder.Services.AddScoped<TenantProvider>();

        builder.Services.AddDbContext<ProjectContext>((sp, options) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var tenantProvider = sp.GetRequiredService<TenantProvider>();

            var tenantId = Environment.GetEnvironmentVariable(variable: "TENANT_ID") ?? tenantProvider.TenantId;

            var connString = config.GetConnectionString(name: "BasicConnection")?.Replace(oldValue: "{tenant}", tenantId);

            if (string.IsNullOrWhiteSpace(connString))
            {
                throw new Exception(message: "Connection string inválida");
            }

            options.UseNpgsql(connString).EnableSensitiveDataLogging().UseSnakeCaseNamingConvention();
        });

        builder.Services.AddAuthentication(x =>
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
                x.WithDarkModeToggle(showDarkModeToggle: true).WithTheme(ScalarTheme.BluePlanet).WithClientButton(showButton: true);

                x.Authentication = new ScalarAuthenticationOptions
                {
                    PreferredSecuritySchemes = ["Bearer "]
                };
            });
        }

        app.UseAuthentication();
        app.UseMiddleware<TenantMiddleware>();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
