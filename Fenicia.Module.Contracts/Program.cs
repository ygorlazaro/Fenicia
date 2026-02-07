using System.Text;

using Fenicia.Common;
using Fenicia.Common.API.Middlewares;
using Fenicia.Common.API.Providers;
using Fenicia.Common.Data.Contexts;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Scalar.AspNetCore;

namespace Fenicia.Module.Contracts;

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

        var key = Encoding.ASCII.GetBytes(configuration["Jwt:Secret"]
                                          ?? throw new InvalidOperationException(TextConstants
                                              .InvalidJwtSecretMessage));

        builder.Services.AddScoped<TenantProvider>();

        builder.Services.AddDbContext<ContractsContext>((sp, o) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var tenantProvider = sp.GetRequiredService<TenantProvider>();

            var tenantId = Environment.GetEnvironmentVariable("TENANT_ID") ?? tenantProvider.TenantId;

            var connString = config.GetConnectionString("Contracts")?.Replace("{tenant}", tenantId);

            if (string.IsNullOrWhiteSpace(connString)) throw new Exception("Connection string inválida");

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

        app.UseWhen(o => o.Request.Path.StartsWithSegments("/contract"),
            appBuilder => appBuilder.UseModuleRequirement("contract"));

        app.MapControllers();

        app.Run();
    }
}