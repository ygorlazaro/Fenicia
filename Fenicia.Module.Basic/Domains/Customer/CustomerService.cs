using Fenicia.Common.Database.Converters.Basic;
using Fenicia.Common.Database.Requests.Basic;
using Fenicia.Common.Database.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Customer;

public class CustomerService(ICustomerRepository customerRepository) : ICustomerService
{
    public async Task<List<CustomerResponse>> GetAllAsync(CancellationToken cancellationToken, int page = 1, int perPage = 1)
    {
        var customers = await customerRepository.GetAllAsync(cancellationToken, page, perPage);

        return CustomerConverter.Convert(customers);
    }

    public async Task<CustomerResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByIdAsync(id, cancellationToken);

        return customer is null ? null : CustomerConverter.Convert(customer);
    }

    public async Task<CustomerResponse?> AddAsync(CustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = CustomerConverter.Convert(request);

        customerRepository.Add(customer);

        await customerRepository.SaveChangesAsync(cancellationToken);

        return CustomerConverter.Convert(customer);
    }

    public async Task<CustomerResponse?> UpdateAsync(CustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = CustomerConverter.Convert(request);

        customerRepository.Update(customer);

        await customerRepository.SaveChangesAsync(cancellationToken);

        return CustomerConverter.Convert(customer);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        customerRepository.Delete(id);

        await customerRepository.SaveChangesAsync(cancellationToken);
    }
}
