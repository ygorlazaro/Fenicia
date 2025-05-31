using System.Text;
using Fenicia.Common;
using Fenicia.Common.Api.Middlewares;
using Fenicia.Common.Api.Providers;
using Fenicia.Module.Basic.Contexts;
using Fenicia.Module.Basic.Repositories;
using Fenicia.Module.Basic.Repositories.Interfaces;
using Fenicia.Module.Basic.Services;
using Fenicia.Module.Basic.Services.Interfaces;
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

        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        var key = Encoding.ASCII.GetBytes(configuration["Jwt:Secret"] ??
                                          throw new InvalidOperationException(TextConstants.InvalidJwtSecret));

        builder.Services.AddScoped<TenantProvider>();
        builder.Services.AddTransient<IStateService, StateService>();
        builder.Services.AddTransient<IStateRepository, StateRepository>();

        builder.Services.AddDbContext<BasicContext>((sp, options) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var tenantProvider = sp.GetRequiredService<TenantProvider>();

            var tenantId = Environment.GetEnvironmentVariable("TENANT_ID") ?? tenantProvider.TenantId;

            var connString = config.GetConnectionString("BasicConnection")?.Replace("{tenant}", tenantId);

            if (string.IsNullOrWhiteSpace(connString))
            {
                throw new Exception("Connection string inválida");
            }

            options
                .UseNpgsql(connString)
                .EnableSensitiveDataLogging()
                .UseSnakeCaseNamingConvention();
        });

        builder.Services.AddAuthentication(x =>
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
                x.WithDarkModeToggle(true)
                    .WithTheme(ScalarTheme.BluePlanet)
                    .WithClientButton(true);

                x.Authentication = new ScalarAuthenticationOptions
                {
                    PreferredSecurityScheme = "Bearer"
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