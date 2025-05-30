namespace Fenicia.Auth.Services.Interfaces;

public interface ICustomerService
{
    Task<Guid?> GetOrCreateByUserIdAsync(Guid userId, Guid companyId);
}