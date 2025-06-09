using System.Text;
using Fenicia.Common;
using Fenicia.Common.Api.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

namespace Fenicia.ERP.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        var key = Encoding.ASCII.GetBytes(
            configuration["Jwt:Secret"]
                ?? throw new InvalidOperationException(TextConstants.InvalidJwtSecret)
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
        app.UseMiddleware<RoleGodRequirementMiddleware>();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
