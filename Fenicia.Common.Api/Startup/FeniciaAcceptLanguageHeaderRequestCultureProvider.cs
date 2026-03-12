using System.Globalization;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace Fenicia.Common.API.Startup;

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