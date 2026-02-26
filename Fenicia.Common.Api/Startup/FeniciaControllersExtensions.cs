using System.Reflection;
using System.Text.Json.Serialization;

using Fenicia.Common.Data;

using FluentValidation;
using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace Fenicia.Common.API.Startup;

public static class FeniciaControllersExtensions
{
    public static WebApplicationBuilder AddFeniciaControllers(this WebApplicationBuilder builder, Assembly? assembly = null)
    {
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

        var targetAssembly = (assembly ?? Assembly.GetEntryAssembly()) ?? throw new InvalidOperationException("Could not determine the assembly to load controllers from.");

        builder.Services.AddControllers()
            .ConfigureApplicationPartManager(manager =>
            {
                manager.ApplicationParts.Clear();
                manager.ApplicationParts.Add(new AssemblyPart(targetAssembly));
            })
            .AddJsonOptions(o =>
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
