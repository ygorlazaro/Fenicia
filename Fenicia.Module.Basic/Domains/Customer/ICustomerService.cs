using Fenicia.Common.Database.Requests.Basic;
using Fenicia.Common.Database.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Customer;

public interface ICustomerService
{
    Task<List<CustomerResponse>> GetAllAsync(CancellationToken cancellationToken, int page = 1, int perPage = 1);

    Task<CustomerResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<CustomerResponse?> AddAsync(CustomerRequest request, CancellationToken cancellationToken);

    Task<CustomerResponse?> UpdateAsync(CustomerRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
