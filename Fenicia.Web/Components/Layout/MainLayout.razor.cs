using Fenicia.Web.Components.Layout.Models;
using Fenicia.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace Fenicia.Web.Components.Layout;

#pragma warning disable CA1031
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
            var doc = System.Text.Json.JsonDocument.Parse(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload)));
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
            var doc = System.Text.Json.JsonDocument.Parse(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload)));
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

    private bool _isLoggedIn;

    private string _userName = string.Empty;

    private string _userRole = string.Empty;

    private string? _userImageUrl;

    private bool _jsAvailable;

    private bool _drawerOpen;

    private bool _companiesLoaded;

    private List<UserCompanyItem> _companies = [];

    private Guid? _selectedCompanyId;

    private string _selectedCompanyName = string.Empty;

    private bool _isLoading;

    [Inject]

    public ICompanyContextService CompanyContext { get; set; } = default!;

    [Inject]

    public IUserProfileNotifier UserProfileNotifier { get; set; } = default!;

    [Inject]

    public ILoadingService LoadingService { get; set; } = default!;

    [Inject]

    public ICompanySelectionState CompanyState { get; set; } = default!;

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
        try
        {
            if (_jsAvailable)
            {
                await RefreshAuthAsync();

                if (_companiesLoaded)
                {
                    return;
                }
                _companiesLoaded = true;

                if (_isLoggedIn)
                {
                    await LoadCompaniesAsync();

                    await RedirectIfCompanyMissingAsync();
                }
                return;
            }

            await JsRuntime.InvokeAsync<string>("storageHelper.get", "auth_token");

            _jsAvailable = true;

            await RefreshAuthAsync();

            if (!_companiesLoaded)
            {
                _companiesLoaded = true;

                if (_isLoggedIn)
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
            if (!_isLoggedIn)
            {
                return Task.CompletedTask;
            }

            if (_selectedCompanyId.HasValue)
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
        if (_jsAvailable)
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
        _drawerOpen = !_drawerOpen;
    }

    private async Task RefreshAuthAsync()
    {
        try
        {
            var token = await JsRuntime.InvokeAsync<string>("storageHelper.get", "auth_token");

            var loggedIn = !string.IsNullOrEmpty(token);

            if (loggedIn != _isLoggedIn)
            {
                _isLoggedIn = loggedIn;

                if (loggedIn)
                {
                    _userName = ParseUserName(token) ?? string.Empty;

                    _userRole = ParseUserRole(token) ?? string.Empty;

                    _userImageUrl = await LoadUserImageAsync();
                }
                else
                {
                    _userName = string.Empty;

                    _userRole = string.Empty;

                    _userImageUrl = null;

                    _selectedCompanyId = null;

                    _selectedCompanyName = string.Empty;

                    _companies = [];

                    _companiesLoaded = false;
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
        if (!_isLoggedIn || _companies.Count > 0)
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
                    _companies = items;

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
                _selectedCompanyId = companyId;

                var match = _companies.FirstOrDefault(c => c.Id == companyId.Value);

                if (match is not null)
                {
                    _selectedCompanyName = match.CompanyName;

                    CompanyState.Set(match.Id, match.CompanyName);

                    await JsRuntime.InvokeVoidAsync("storageHelper.setCookie", "selected_company_id", companyId.Value.ToString(), 7);
                }
                else
                {
                    _selectedCompanyId = null;

                    _selectedCompanyName = string.Empty;

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
        _isLoading = refIsLoading;

        InvokeAsync(StateHasChanged);
    }

    private string GetRequestedPath()
    {
        var uri = new Uri(NavigationManager.Uri);

        var path = uri.AbsolutePath;

        return string.IsNullOrEmpty(path) || path == "/" ? "dashboard" : path.TrimStart('/');
    }

}
