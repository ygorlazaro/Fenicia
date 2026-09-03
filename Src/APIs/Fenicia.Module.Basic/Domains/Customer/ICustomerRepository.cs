using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Basic.Domains.Customer;

public interface ICustomerRepository : IRepository<CustomerModel>
{
    Task<CustomerModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<CustomerModel>> GetAllWithDetailsAsync(
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default);
}