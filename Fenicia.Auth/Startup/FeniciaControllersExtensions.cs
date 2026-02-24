using System.Text;
using System.Text.Json.Serialization;

using Fenicia.Common;
using Fenicia.Common.Data;

using FluentValidation;
using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Fenicia.Auth.Startup;

public static class FeniciaControllersExtensions
{
    public static WebApplicationBuilder AddFeniciaControllers(this WebApplicationBuilder builder, IConfiguration configuration)
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

        builder.Services.Configure<ApiBehaviorOptions>(o =>
        {
            o.InvalidModelStateResponseFactory = c =>
            {
                var problemDetails = new ValidationProblemDetails(c.ModelState)
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Title = "Um ou mais erros de validação ocorreram.",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = c.HttpContext.Request.Path
                };

                return new BadRequestObjectResult(problemDetails)
                {
                    ContentTypes = { "application/problem+json" }
                };
            };
        });

        builder.Services.AddControllers().AddJsonOptions(o =>
        {
            o.JsonSerializerOptions.AllowTrailingCommas = false;
            o.JsonSerializerOptions.MaxDepth = 0;
            o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        builder.Services.AddOpenApi();

        builder.Services.AddFluentValidationAutoValidation()
            .AddValidatorsFromAssemblyContaining<BaseModel>();

        return builder;
    }
}