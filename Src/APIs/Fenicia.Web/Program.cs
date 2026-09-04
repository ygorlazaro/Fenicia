using Fenicia.Web;
using Fenicia.Web.Components;
using Fenicia.Web.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient("FeniciaAuth", client =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5000";
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<CompanyHeaderHandler>();

builder.Services.AddHttpClient("FeniciaBasic", client =>
{
    var apiBaseUrl = builder.Configuration["BasicApiBaseUrl"] ?? "http://localhost:5083";
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<CompanyHeaderHandler>();

builder.Services.AddScoped<ICompanyContextService, CompanyContextService>();
builder.Services.AddScoped<CompanyHeaderHandler>();
builder.Services.AddScoped<NotificationService>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddScoped<IAuthStateService, AuthStateService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
