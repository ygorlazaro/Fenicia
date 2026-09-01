using System.Globalization;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace Fenicia.Common.API.Startup;

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

        var languages = acceptLanguage.Split(',');
        var requestedCultures = new List<CultureInfo>();

        foreach (var language in languages)
        {
            var cultureName = language.Split(';').FirstOrDefault()?.Trim();
            if (string.IsNullOrEmpty(cultureName))
            {
                continue;
            }

            try
            {
                requestedCultures.Add(new CultureInfo(cultureName));
            }
            catch (CultureNotFoundException)
            {
            }
        }

        if (requestedCultures.Count <= 0)
        {
            return Task.FromResult<ProviderCultureResult?>(null);
        }

        var result = new ProviderCultureResult(requestedCultures[0].Name);
        return Task.FromResult<ProviderCultureResult?>(result);
    }
}
