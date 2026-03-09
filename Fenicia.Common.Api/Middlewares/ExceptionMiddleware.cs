using System.Globalization;

using Fenicia.Common.Localization;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Common.API.Middlewares;

public class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            // Set culture based on Accept-Language header
            var acceptLanguage = context.Request.Headers.AcceptLanguage.FirstOrDefault();
            if (!string.IsNullOrEmpty(acceptLanguage))
            {
                SetCulture(acceptLanguage);
            }

            var problem = new ProblemDetails
            {
                Title = ExceptionMessages.InternalError,
                Status = context.Response.StatusCode,
                Detail = ex.Message,
                Instance = context.Request.Path
            };

            await context.Response.WriteAsJsonAsync(problem);
        }
    }

    private static void SetCulture(string acceptLanguage)
    {
        // Parse Accept-Language header (e.g., "pt-BR,pt;q=0.9,en;q=0.8")
        var primaryLanguage = acceptLanguage.Split(',').FirstOrDefault()?.Split(';').FirstOrDefault()?.Trim();

        if (!string.IsNullOrEmpty(primaryLanguage))
        {
            try
            {
                var culture = new CultureInfo(primaryLanguage);
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
            catch (CultureNotFoundException)
            {
                // If the culture is not supported, fall back to default (English)
                var culture = new CultureInfo("en-US");
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
        }
    }
}
