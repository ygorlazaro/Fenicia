using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Customer;

public class CustomerService(ICustomerRepository customerRepository) : ICustomerService
{
    public async Task<List<CustomerResponse>> GetAllAsync(CancellationToken ct, int page = 1, int perPage = 1)
    {
        var customers = await customerRepository.GetAllAsync(ct, page, perPage);

        return [.. customers.Select(c => new CustomerResponse(c))];
    }

    public async Task<CustomerResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var customer = await customerRepository.GetByIdAsync(id, ct);

        return customer is null ? null : new CustomerResponse(customer);
    }

    public async Task<CustomerResponse?> AddAsync(CustomerRequest request, CancellationToken ct)
    {
        var customer = new CustomerModel(request);

        customerRepository.Add(customer);

        await customerRepository.SaveChangesAsync(ct);

        return new CustomerResponse(customer);
    }

    public async Task<CustomerResponse?> UpdateAsync(CustomerRequest request, CancellationToken ct)
    {
        var customer = new CustomerModel(request);

        customerRepository.Update(customer);

        await customerRepository.SaveChangesAsync(ct);

        return new CustomerResponse(customer);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        await customerRepository.DeleteAsync(id, ct);

        await customerRepository.SaveChangesAsync(ct);
    }
}