using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Fenicia.Common.API.Startup;

public static class FeniciaAuthenticationExtensions
{
    public static WebApplicationBuilder AddFeniciaAuthentication(this WebApplicationBuilder builder,IConfiguration configuration)
    {
        var key = Encoding.ASCII.GetBytes(configuration["Jwt:Secret"]
                                          ?? throw new InvalidOperationException(TextConstants
                                              .InvalidJwtSecretMessage));
        
        builder.Services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.RequireHttpsMetadata = false;
            o.SaveToken = true;
            o.ClaimsIssuer = "AuthService";
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        return builder;
    }
}