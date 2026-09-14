namespace Fenicia.Web.Services.Interfaces;

public interface IAuthStateService
{
    Task<bool> IsAuthenticatedAsync();

    Task<string?> GetUserNameAsync();

    Task SetTokenAsync(string token, int expiryHours = 3);

    Task ClearTokenAsync();
}
