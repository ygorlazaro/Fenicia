using System.Globalization;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Fenicia.Common.API.Startup;

public static class FeniciaLocalizationExtensions
{
    public static WebApplicationBuilder AddFeniciaLocalization(this WebApplicationBuilder builder)
    {
        builder.Services.AddLocalization();

        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("pt-BR"),
                new CultureInfo("es-ES")
            };

            options.DefaultRequestCulture = new RequestCulture("en-US");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;

            // Add custom header provider for Accept-Language
            options.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());
        });

        return builder;
    }

    public static WebApplication UseFeniciaLocalization(this WebApplication app)
    {
        var options = app.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(options.Value);

        return app;
    }
}

/// <summary>
/// Custom request culture provider that reads the Accept-Language header.
/// </summary>
public class AcceptLanguageHeaderRequestCultureProvider : RequestCultureProvider
{
    public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        if (httpContext is null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

        var acceptLanguage = httpContext.Request.Headers.AcceptLanguage.FirstOrDefault();

        if (string.IsNullOrEmpty(acceptLanguage))
        {
            return Task.FromResult<ProviderCultureResult?>(null);
        }

        // Parse Accept-Language header (e.g., "pt-BR,pt;q=0.9,en;q=0.8")
        var languages = acceptLanguage.Split(',');
        var requestedCultures = new List<CultureInfo>();

        foreach (var language in languages)
        {
            var cultureName = language.Split(';').FirstOrDefault()?.Trim();
            if (!string.IsNullOrEmpty(cultureName))
            {
                try
                {
                    requestedCultures.Add(new CultureInfo(cultureName));
                }
                catch (CultureNotFoundException)
                {
                    // Ignore unsupported cultures
                }
            }
        }

        if (requestedCultures.Count > 0)
        {
            var result = new ProviderCultureResult(requestedCultures[0].Name);
            return Task.FromResult<ProviderCultureResult?>(result);
        }

        return Task.FromResult<ProviderCultureResult?>(null);
    }
}
