using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Services.Interfaces;

namespace Fenicia.Auth.Services;

public class CustomerService(ICustomerRepository customerRepository) : ICustomerService
{
    public async Task<Guid?> GetOrCreateByUserIdAsync(Guid userId, Guid companyId)
    {
        var customer = await customerRepository.GetByUserIdAsync(userId, companyId);

        return customer?.Id ?? await customerRepository.CreateNewCustomerAsync(userId, companyId);
    }
}