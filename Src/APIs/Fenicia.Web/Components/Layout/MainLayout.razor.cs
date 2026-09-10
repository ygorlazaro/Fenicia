using Fenicia.Web.Components.Layout.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

#pragma warning disable CA1031
#pragma warning disable SA1204
#pragma warning disable SA1513
#pragma warning disable SA1201

namespace Fenicia.Web.Components.Layout;

public partial class MainLayout : IDisposable
{
    private static string? ParseUserName(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length < 2)
            {
                return null;
            }
            var payload = parts[1].Replace('-', '+').Replace('_', '/');
            switch (payload.Length % 4)
            {
                case 2:
                    payload += "==";
                    break;
                case 3:
                    payload += "=";
                    break;
            }
            using var doc = System.Text.Json.JsonDocument.Parse(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload)));
            return doc.RootElement.TryGetProperty("unique_name", out var n) ? n.GetString() : null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MainLayout] Parse error: {ex.Message}");
            return null;
        }
    }

    private static string? ParseUserRole(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length < 2)
            {
                return null;
            }
            var payload = parts[1].Replace('-', '+').Replace('_', '/');
            switch (payload.Length % 4)
            {
                case 2:
                    payload += "==";
                    break;
                case 3:
                    payload += "=";
                    break;
            }
            using var doc = System.Text.Json.JsonDocument.Parse(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload)));
            if (doc.RootElement.TryGetProperty("role", out var roleElement))
            {
                var roleJson = roleElement.GetString();
                if (!string.IsNullOrEmpty(roleJson))
                {
                    if (roleJson.StartsWith("["))
                    {
                        var roles = System.Text.Json.JsonSerializer.Deserialize<List<string>>(roleJson);
                        return roles is { Count: > 0 } ? roles[0] : null;
                    }
                    return roleJson;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MainLayout] Parse role error: {ex.Message}");
        }
        return null;
    }

    private bool isLoggedIn;
    private string userName = string.Empty;
    private string userRole = string.Empty;
    private string? userImageUrl;
    private bool jsAvailable;
    private bool drawerOpen;
    private bool companiesLoaded;
    private List<UserCompanyItem> companies = [];
    private Guid? selectedCompanyId;
    private string selectedCompanyName = string.Empty;
    private bool isLoading;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
        UserProfileNotifier.Changed -= OnUserProfileChanged;
        LoadingService.LoadingChanged -= OnLoadingChanged;
    }

    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += OnLocationChanged;
        UserProfileNotifier.Changed += OnUserProfileChanged;
        LoadingService.LoadingChanged += OnLoadingChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (jsAvailable)
        {
            await RefreshAuthAsync();
            if (companiesLoaded)
            {
                return;
            }
            companiesLoaded = true;
            if (isLoggedIn)
            {
                await LoadCompaniesAsync();
                await RedirectIfCompanyMissingAsync();
            }
            return;
        }

        try
        {
            await JsRuntime.InvokeAsync<string>("storageHelper.get", "auth_token");
            jsAvailable = true;
            await RefreshAuthAsync();
            if (!companiesLoaded)
            {
                companiesLoaded = true;
                if (isLoggedIn)
                {
                    await LoadCompaniesAsync();
                    await RedirectIfCompanyMissingAsync();
                }
            }
        }
        catch (Exception ex)
        {
            if (ex is JSException jsEx && jsEx.Message.Contains("statically rendered"))
            {
                return;
            }
            Console.WriteLine($"[MainLayout] OnAfterRenderAsync error: {ex.GetType().Name}: {ex.Message}");
        }
    }

    private Task RedirectIfCompanyMissingAsync()
    {
        try
        {
            if (!isLoggedIn)
            {
                return Task.CompletedTask;
            }

            if (selectedCompanyId.HasValue)
            {
                return Task.CompletedTask;
            }

            var destiny = GetRequestedPath();
            var query = string.IsNullOrEmpty(destiny) ? string.Empty : $"?destiny={Uri.EscapeDataString(destiny)}";
            NavigationManager.NavigateTo($"/company{query}", true);
            return Task.CompletedTask;
        }
        catch
        {
            return Task.CompletedTask;
        }
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        if (jsAvailable)
        {
            _ = RefreshAuthAsync();
        }
    }

    private void OnUserProfileChanged()
    {
        _ = RefreshAuthAsync();
    }

    private void ToggleDrawer()
    {
        drawerOpen = !drawerOpen;
    }

    private async Task RefreshAuthAsync()
    {
        try
        {
            var token = await JsRuntime.InvokeAsync<string>("storageHelper.get", "auth_token");
            var loggedIn = !string.IsNullOrEmpty(token);
            if (loggedIn != isLoggedIn)
            {
                isLoggedIn = loggedIn;
                if (loggedIn)
                {
                    userName = ParseUserName(token) ?? string.Empty;
                    userRole = ParseUserRole(token) ?? string.Empty;
                    userImageUrl = await LoadUserImageAsync();
                }
                else
                {
                    userName = string.Empty;
                    userRole = string.Empty;
                    userImageUrl = null;
                    selectedCompanyId = null;
                    selectedCompanyName = string.Empty;
                    companies = [];
                    companiesLoaded = false;
                }
                await InvokeAsync(StateHasChanged);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MainLayout] Auth check error: {ex.GetType().Name}: {ex.Message}");
        }
    }

    private async Task<string?> LoadUserImageAsync()
    {
        try
        {
            var token = await JsRuntime.InvokeAsync<string>("storageHelper.get", "auth_token");
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var client = HttpClientFactory.CreateClient("FeniciaSocialNetwork");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var companyId = await CompanyContext.GetSelectedCompanyIdAsync();
            if (companyId.HasValue)
            {
                client.DefaultRequestHeaders.Remove("CompanyId");
                client.DefaultRequestHeaders.Add("CompanyId", companyId.Value.ToString());
            }

            var response = await client.GetAsync("/profile");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var profile = System.Text.Json.JsonSerializer.Deserialize<SocialProfileRef>(
                    json,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return profile?.ImageUrl;
            }
        }
        catch
        {
        }
        return null;
    }

    private async Task LoadCompaniesAsync()
    {
        if (!isLoggedIn || companies.Count > 0)
        {
            return;
        }

        await InvokeAsync(StateHasChanged);

        try
        {
            var userId = await CompanyContext.GetUserIdAsync();
            var token = await CompanyContext.GetTokenAsync();
            var client = HttpClientFactory.CreateClient("FeniciaAuth");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var storedCompanyId = await CompanyContext.GetSelectedCompanyIdAsync();
            if (storedCompanyId.HasValue)
            {
                client.DefaultRequestHeaders.Remove("CompanyId");
                client.DefaultRequestHeaders.Add("CompanyId", storedCompanyId.Value.ToString());
            }

            var response = await client.GetAsync($"/user/{userId}/company");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var items = System.Text.Json.JsonSerializer.Deserialize<List<UserCompanyItem>>(
                    json,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (items is not null)
                {
                    companies = items;
                    await LoadSelectedCompanyAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MainLayout] Load error: {ex.Message}");
        }
        finally
        {
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task LoadSelectedCompanyAsync()
    {
        try
        {
            var companyId = await CompanyContext.GetSelectedCompanyIdAsync();
            if (companyId.HasValue)
            {
                selectedCompanyId = companyId;
                var match = companies.FirstOrDefault(c => c.Id == companyId.Value);
                if (match is not null)
                {
                    selectedCompanyName = match.CompanyName;
                    CompanyState.Set(match.Id, match.CompanyName);
                    await JsRuntime.InvokeVoidAsync("storageHelper.setCookie", "selected_company_id", companyId.Value.ToString(), 7);
                }
                else
                {
                    selectedCompanyId = null;
                    selectedCompanyName = string.Empty;
                    await CompanyContext.SetSelectedCompanyIdAsync(null);
                    await JsRuntime.InvokeVoidAsync("storageHelper.removeCookie", "selected_company_id");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MainLayout] Load selected error: {ex.Message}");
        }
    }

    private void OnLoadingChanged(bool refIsLoading)
    {
        isLoading = refIsLoading;
        InvokeAsync(StateHasChanged);
    }

    private string GetRequestedPath()
    {
        var uri = new Uri(NavigationManager.Uri);
        var path = uri.AbsolutePath;
        return string.IsNullOrEmpty(path) || path == "/" ? "dashboard" : path.TrimStart('/');
    }
}
