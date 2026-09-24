using Fenicia.Common;
using Fenicia.Common.DTOs.Basic.Customer;
using Fenicia.Common.DTOs.Basic.DataSource;

namespace Fenicia.Module.Basic.Domains.Customer.Interfaces;

public interface ICustomerService
{
    Task<Pagination<List<GetAllCustomerResponse>>> GetAllAsync(
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default);

    Task<List<GetAllCustomerForDataSourceResponse>> GetAllForDataSourceAsync(
        CancellationToken cancellationToken = default);

    Task<GetCustomerByIdResponse?> GetByIdAsync(
        GetCustomerByIdQuery query,
        CancellationToken cancellationToken = default);

    Task<AddCustomerResponse> AddAsync(
        AddCustomerRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<UpdateCustomerResponse?> UpdateAsync(
        UpdateCustomerRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(DeleteCustomerRequest command, Guid companyId, CancellationToken cancellationToken = default);

    Task<CustomerInsightsResponse> GetInsightsAsync(
        GetCustomerInsightsQuery query,
        CancellationToken cancellationToken = default);

    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}
