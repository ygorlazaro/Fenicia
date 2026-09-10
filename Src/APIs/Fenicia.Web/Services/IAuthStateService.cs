namespace Fenicia.Web.Services;

public interface IAuthStateService
{
    Task<bool> IsAuthenticatedAsync();

    Task<string?> GetUserNameAsync();

    Task SetTokenAsync(string token, int expiryHours = 3);

    Task ClearTokenAsync();
}