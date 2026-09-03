using System.Text.Json;
using Microsoft.JSInterop;

namespace Fenicia.Web.Services;

public interface IAuthStateService
{
    Task<bool> IsAuthenticatedAsync();

    Task<string?> GetUserNameAsync();

    Task SetTokenAsync(string token, int expiryHours = 3);

    Task ClearTokenAsync();
}

public class AuthStateService(IJSRuntime jsRuntime) : IAuthStateService
{
    private bool _isAuthenticated;
    private string? _userName;

    public async Task<bool> IsAuthenticatedAsync()
    {
        if (!_isAuthenticated)
        {
            var token = await jsRuntime.InvokeAsync<string>("storageHelper.get", "auth_token");
            _isAuthenticated = !string.IsNullOrEmpty(token);
            if (_isAuthenticated)
            {
                _userName = GetUserNameFromToken(token);
            }
        }

        return _isAuthenticated;
    }

    public async Task<string?> GetUserNameAsync()
    {
        if (!_isAuthenticated)
        {
            await IsAuthenticatedAsync();
        }

        return _userName;
    }

    public async Task SetTokenAsync(string token, int expiryHours = 3)
    {
        _isAuthenticated = true;
        _userName = GetUserNameFromToken(token);
        await jsRuntime.InvokeVoidAsync("storageHelper.set", "auth_token", token);
    }

    public async Task ClearTokenAsync()
    {
        _isAuthenticated = false;
        _userName = null;
        await jsRuntime.InvokeVoidAsync("storageHelper.remove", "auth_token");
    }

    private static string? GetUserNameFromToken(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length < 2)
            {
                return null;
            }

            var payload = parts[1];
            payload = payload.Replace('-', '+').Replace('_', '/');
            switch (payload.Length % 4)
            {
                case 2:
                    payload += "==";
                    break;
                case 3:
                    payload += "=";
                    break;
            }

            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("unique_name", out var nameElement))
            {
                return nameElement.GetString();
            }
        }
        catch (FormatException)
        {
            // ignore decode errors
        }
        catch (JsonException)
        {
            // ignore JSON parse errors
        }

        return null;
    }
}
