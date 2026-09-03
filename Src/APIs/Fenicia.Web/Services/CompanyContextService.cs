using Microsoft.JSInterop;

namespace Fenicia.Web.Services;

public interface ICompanyContextService
{
    Task<Guid?> GetSelectedCompanyIdAsync();

    Task SetSelectedCompanyIdAsync(Guid? companyId);

    Task<Guid> GetUserIdAsync();

    Task<string?> GetTokenAsync();

    Task<bool> IsAuthenticatedAsync();
}

public class CompanyContextService(IJSRuntime jsRuntime) : ICompanyContextService
{
    private const string CompanyIdKey = "selected_company_id";

    public async Task<Guid?> GetSelectedCompanyIdAsync()
    {
        var value = await jsRuntime.InvokeAsync<string>("storageHelper.get", CompanyIdKey);
        if (Guid.TryParse(value, out var companyId))
        {
            return companyId;
        }

        return null;
    }

    public async Task SetSelectedCompanyIdAsync(Guid? companyId)
    {
        if (companyId.HasValue)
        {
            await jsRuntime.InvokeVoidAsync("storageHelper.set", CompanyIdKey, companyId.Value.ToString());
        }
        else
        {
            await jsRuntime.InvokeVoidAsync("storageHelper.remove", CompanyIdKey);
        }
    }

    public async Task<Guid> GetUserIdAsync()
    {
        var token = await jsRuntime.InvokeAsync<string>("storageHelper.get", "auth_token");
        if (string.IsNullOrEmpty(token))
        {
            throw new UnauthorizedAccessException();
        }

        var parts = token.Split('.');
        if (parts.Length < 2)
        {
            throw new UnauthorizedAccessException();
        }

        var payload = parts[1];
        payload = payload.Replace('-', '+').Replace('_', '/');
        switch (payload.Length % 4)
        {
            case 2: payload += "=="; break;
            case 3: payload += "="; break;
        }

        var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
        using var doc = System.Text.Json.JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("userId", out var userIdElement) &&
            Guid.TryParse(userIdElement.GetString(), out var userId))
        {
            return userId;
        }

        if (doc.RootElement.TryGetProperty("sub", out var subElement) &&
            Guid.TryParse(subElement.GetString(), out var subId))
        {
            return subId;
        }

        throw new UnauthorizedAccessException();
    }

    public async Task<string?> GetTokenAsync()
    {
        return await jsRuntime.InvokeAsync<string>("storageHelper.get", "auth_token");
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }
}
