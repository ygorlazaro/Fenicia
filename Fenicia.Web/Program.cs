using Blazored.LocalStorage;

using Fenicia.Web.Components;
using Fenicia.Web.Providers;
using Fenicia.Web.Providers.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<HttpClient>();
builder.Services.AddScoped<ApiAuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<TokenProvider>();
builder.Services.AddScoped<RegisterProvider>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
