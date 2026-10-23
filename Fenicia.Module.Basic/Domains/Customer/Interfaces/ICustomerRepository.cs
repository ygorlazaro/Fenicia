using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;

/// <summary>
/// Fenicia.Module.Basic/Domains/Customer/Interfaces/ICustomerRepository.cs implementation.
/// </summary>

namespace Fenicia.Module.Basic.Domains.Customer.Interfaces;

public interface ICustomerRepository : IRepository<CustomerModel>
{
    Task<CustomerModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<CustomerModel>> GetAllWithDetailsAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default);
}