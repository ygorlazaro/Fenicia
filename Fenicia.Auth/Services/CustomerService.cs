using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Services.Interfaces;

namespace Fenicia.Auth.Services;

public class CustomerService(ICustomerRepository customerRepository) : ICustomerService
{
    public async Task<Guid?> GetOrCreateByUserIdAsync(Guid userId, Guid companyId)
    {
        var customer = await customerRepository.GetByUserIdAsync(userId, companyId);

        if (customer is not null)
        {
            return customer.Id;
        }
        
        return await customerRepository.CreateNewCustomerAsync(userId, companyId);
    }
}