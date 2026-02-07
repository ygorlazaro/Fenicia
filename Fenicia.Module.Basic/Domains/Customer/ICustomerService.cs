using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Customer;

public interface ICustomerService
{
    Task<List<CustomerResponse>> GetAllAsync(CancellationToken ct, int page = 1, int perPage = 1);

    Task<CustomerResponse?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<CustomerResponse?> AddAsync(CustomerRequest request, CancellationToken ct);

    Task<CustomerResponse?> UpdateAsync(CustomerRequest request, CancellationToken ct);

    Task DeleteAsync(Guid id, CancellationToken ct);
}
