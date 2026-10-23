using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common;

namespace Fenicia.Module.Basic.Domains.Customer.Interfaces;

public interface ICustomerRepository : IRepository<CustomerModel>
{
    Task<CustomerModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<CustomerModel>> GetAllWithDetailsAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default);
}