using System.Text;
using Fenicia.Common;
using Fenicia.Common.Api.Middlewares;
using Fenicia.Common.Api.Providers;
using Fenicia.Module.HR.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

namespace Fenicia.Module.HR;

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

        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        var key = Encoding.ASCII.GetBytes(
            configuration["Jwt:Secret"]
                ?? throw new InvalidOperationException(TextConstants.InvalidJwtSecret)
        );

        builder.Services.AddScoped<TenantProvider>();

        builder.Services.AddDbContext<HrContext>(
            (sp, options) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var tenantProvider = sp.GetRequiredService<TenantProvider>();

                var tenantId =
                    Environment.GetEnvironmentVariable("TENANT_ID") ?? tenantProvider.TenantId;

                var connString = config
                    .GetConnectionString("HrConnection")
                    ?.Replace("{tenant}", tenantId);

                if (string.IsNullOrWhiteSpace(connString))
                {
                    throw new Exception("Connection string inválida");
                }

                options
                    .UseNpgsql(connString)
                    .EnableSensitiveDataLogging()
                    .UseSnakeCaseNamingConvention();
            }
        );

        builder
            .Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
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
                    ValidateLifetime = true,
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
                x.WithDarkModeToggle(true).WithTheme(ScalarTheme.BluePlanet).WithClientButton(true);

                x.Authentication = new ScalarAuthenticationOptions
                {
                    PreferredSecurityScheme = "Bearer",
                };
            });
        }

        app.UseAuthentication();
        app.UseMiddleware<TenantMiddleware>();
        app.UseAuthorization();

        app.UseWhen(
            context => context.Request.Path.StartsWithSegments("/hr"),
            appBuilder => appBuilder.UseModuleRequirement("hr")
        );

        app.MapControllers();

        app.Run();
    }
}
