using Fenicia.Common;
using Fenicia.Common.DTOs.Basic.Customer;
using Fenicia.Common.DTOs.Basic.DataSource;

namespace Fenicia.Module.Basic.Domains.Customer.Interfaces;

public interface ICustomerService
{
    Task<Pagination<List<CustomerResponse>>> GetAllAsync(
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default);

    Task<List<CustomerDataSourceResponse>> GetAllForDataSourceAsync(
        CancellationToken cancellationToken = default);

    Task<CustomerResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<CustomerResponse> AddAsync(
        CustomerRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<CustomerResponse?> UpdateAsync(
        Guid id,
        CustomerRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, Guid companyId, CancellationToken cancellationToken = default);

    Task<CustomerInsightsResponse> GetInsightsAsync(
        CustomerInsightsRequest query,
        CancellationToken cancellationToken = default);

    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}