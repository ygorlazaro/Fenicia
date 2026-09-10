namespace Fenicia.Web.Services;

public interface ICompanyContextService
{
    Task<Guid?> GetSelectedCompanyIdAsync();

    Task SetSelectedCompanyIdAsync(Guid? companyId);

    Task<string?> GetSelectedCompanyNameAsync();

    Task SetSelectedCompanyNameAsync(string? companyName);

    Task<Guid> GetUserIdAsync();

    Task<string?> GetTokenAsync();

    Task<bool> IsAuthenticatedAsync();
}