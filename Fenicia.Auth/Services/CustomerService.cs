using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Services.Interfaces;

namespace Fenicia.Auth.Services;

public class CustomerService(ILogger<CustomerService> logger, ICustomerRepository customerRepository) : ICustomerService
{
    public async Task<Guid?> GetOrCreateByUserIdAsync(Guid userId, Guid companyId)
    {
        logger.LogInformation($"Getting customer by user id {userId}");
        var customer = await customerRepository.GetByUserIdAsync(userId, companyId);

        return customer?.Id ?? await customerRepository.CreateNewCustomerAsync(userId, companyId);
    }
}