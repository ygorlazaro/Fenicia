using AspNetCoreRateLimit;

using Fenicia.Common.API.Middlewares;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

using Scalar.AspNetCore;

using Serilog;

namespace Fenicia.Common.API.Startup;

public static class FeniciaStartExtensions
{
    public static void Start(this WebApplicationBuilder builder)
    {
        var app = builder.Build();

        app.UseMiddleware<ExceptionMiddleware>();
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<WideEventMiddleware>();
        app.UseResponseCompression();

        app.UseSerilogRequestLogging();

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

        app.UseHttpsRedirection();
        app.UseCors(app.Environment.IsDevelopment() ? "DevCors" : "RestrictedCors");

        app.UseHsts();
        app.UseXContentTypeOptions();
        app.UseReferrerPolicy(o => o.NoReferrer());
        app.UseXXssProtection(o => o.EnabledWithBlockMode());
        app.UseXfo(o => o.Deny());

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseIpRateLimiting();
        app.MapControllers();

        app.Run();
    }
}