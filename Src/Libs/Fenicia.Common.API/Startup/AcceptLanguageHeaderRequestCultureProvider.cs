using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace Fenicia.Common.API.Startup;

internal sealed class AcceptLanguageHeaderRequestCultureProvider : RequestCultureProvider
{
    public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var acceptLanguage = httpContext.Request.Headers.AcceptLanguage.FirstOrDefault();

        if (string.IsNullOrEmpty(acceptLanguage))
        {
            return Task.FromResult<ProviderCultureResult?>(null);
        }

        var languages = acceptLanguage.Split(',');
        var requestedCultures = new List<CultureInfo>();

        foreach (var cultureName in languages.Select(language => language.Split(';').FirstOrDefault()?.Trim()).Where(cultureName => !string.IsNullOrEmpty(cultureName)))
        {
            try
            {
                requestedCultures.Add(new CultureInfo(cultureName ?? throw new InvalidOperationException()));
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