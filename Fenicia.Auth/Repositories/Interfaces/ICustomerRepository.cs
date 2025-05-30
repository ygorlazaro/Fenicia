using Fenicia.Auth.Contexts.Models;

namespace Fenicia.Auth.Repositories.Interfaces;

public interface ICustomerRepository
{
    Task<CustomerModel?> GetByUserIdAsync(Guid userId, Guid companyId);
    Task<Guid?> CreateNewCustomerAsync(Guid userId, Guid companyId);
}