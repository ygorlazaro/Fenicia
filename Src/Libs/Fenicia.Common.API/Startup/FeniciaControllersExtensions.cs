using System.Text.Json.Serialization;
using Fenicia.Common.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Fenicia.Common.API.Startup;

public static class FeniciaControllersExtensions
{
    public static WebApplicationBuilder AddFeniciaControllers(
        this WebApplicationBuilder builder)
    {
        builder.Services.Configure<ApiBehaviorOptions>(o =>
        {
            o.InvalidModelStateResponseFactory = c =>
            {
                var problemDetails = new ValidationProblemDetails(c.ModelState)
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Title = ExceptionMessages.InvalidRequest,
                    Status = StatusCodes.Status400BadRequest,
                    Instance = c.HttpContext.Request.Path
                };

                return new BadRequestObjectResult(problemDetails) { ContentTypes = { "application/problem+json" } };
            };
        });

        builder.Services.AddControllers().AddJsonOptions(o =>
        {
            o.JsonSerializerOptions.AllowTrailingCommas = false;
            o.JsonSerializerOptions.MaxDepth = 0;
            o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddOpenApi();

        return builder;
    }
}