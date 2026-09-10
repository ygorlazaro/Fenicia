using Fenicia.Web;
using Fenicia.Web.Components;
using Fenicia.Web.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient(
        "FeniciaAuth",
        client =>
        {
            var apiBaseUrl = builder.Configuration["services:authapi:http:0"] ?? "http://localhost:5000";
            client.BaseAddress = new Uri(apiBaseUrl);
        })
    .AddHttpMessageHandler<CompanyHeaderHandler>()
    .AddHttpMessageHandler<LoadingHandler>();

builder.Services.AddHttpClient(
        "FeniciaBasic",
        client =>
        {
            var apiBaseUrl = builder.Configuration["services:basic:http:0"] ?? "http://localhost:5083";
            client.BaseAddress = new Uri(apiBaseUrl);
        })
    .AddHttpMessageHandler<CompanyHeaderHandler>()
    .AddHttpMessageHandler<LoadingHandler>();

builder.Services.AddHttpClient(
        "FeniciaSocialNetwork",
        client =>
        {
            var apiBaseUrl = builder.Configuration["services:socialnetwork:http:0"] ?? "http://localhost:5026";
            client.BaseAddress = new Uri(apiBaseUrl);
        })
    .AddHttpMessageHandler<CompanyHeaderHandler>()
    .AddHttpMessageHandler<LoadingHandler>();

builder.Services.AddHttpClient(
        "FeniciaProjects",
        client =>
        {
            var apiBaseUrl = builder.Configuration["services:projects:http:0"] ?? "http://localhost:5144";
            client.BaseAddress = new Uri(apiBaseUrl);
        })
    .AddHttpMessageHandler<CompanyHeaderHandler>()
    .AddHttpMessageHandler<LoadingHandler>();

builder.Services.AddScoped<ICompanyContextService, CompanyContextService>();
builder.Services.AddScoped<ICompanySelectionState, CompanySelectionState>();
builder.Services.AddScoped<ICompanyChangeNotifier, CompanyChangeNotifier>();
builder.Services.AddScoped<IUserProfileNotifier, UserProfileNotifier>();
builder.Services.AddScoped<CompanyHeaderHandler>();
builder.Services.AddSingleton<ILoadingService, LoadingService>();
builder.Services.AddTransient<LoadingHandler>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddScoped<IAuthStateService, AuthStateService>();
builder.Services.AddScoped<ICrudClient, CrudClient>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
