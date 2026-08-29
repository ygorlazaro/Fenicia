using System.Globalization;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Fenicia.Common.API.Startup;

public static class FeniciaLocalizationExtensions
{
    public static WebApplicationBuilder AddFeniciaLocalization(this WebApplicationBuilder builder)
    {
        builder.Services.AddLocalization();

        builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("pt-BR"), new CultureInfo("es-ES") };

        options.DefaultRequestCulture = new RequestCulture("en-US");
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;

        options.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());
    });

        return builder;
    }

    public static WebApplication UseFeniciaLocalization(this WebApplication app)
    {
        var options = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(options.Value);

        return app;
    }
}
