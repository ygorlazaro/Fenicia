using Blazored.LocalStorage;

using Fenicia.Web.Components;
using Fenicia.Web.Providers.Auth;

using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Log the ApiSettings:BaseUrl value for debugging
var apiBaseUrl = builder.Configuration[key: "ApiSettings:BaseUrl"];
Console.WriteLine($"ApiSettings:BaseUrl = {apiBaseUrl}");

// Configure Static Web Assets
StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Configure HttpClient for TokenProvider
builder.Services.AddHttpClient<TokenProvider>(client => { client.BaseAddress = new Uri(builder.Configuration[key: "ApiSettings:BaseUrl"] ?? "http://localhost:5000/"); });
builder.Services.AddScoped<TokenProvider>();

builder.Services.AddHttpClient<SignUpProvider>(client => { client.BaseAddress = new Uri(builder.Configuration[key: "ApiSettings:BaseUrl"] ?? "http://localhost:5000/"); });
builder.Services.AddScoped<SignUpProvider>();

builder.Services.AddHttpClient<CompanyProvider>(client => { client.BaseAddress = new Uri(builder.Configuration[key: "ApiSettings:BaseUrl"] ?? "http://localhost:5000/"); });
builder.Services.AddScoped<CompanyProvider>();
builder.Services.AddBlazoredLocalStorage();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(errorHandlingPath: "/Error", createScopeForErrors: true);
}

// Enable serving static files
app.UseStaticFiles();

// Configure additional static file middleware for wwwroot
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, path2: "wwwroot")),
    RequestPath = ""
});

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
