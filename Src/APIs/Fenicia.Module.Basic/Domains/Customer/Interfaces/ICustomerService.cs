using Fenicia.Common;
using Fenicia.Module.Basic.Domains.Customer.DTOs;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;

namespace Fenicia.Module.Basic.Domains.Customer.Interfaces;

public interface ICustomerService
{
    Task<Pagination<List<GetAllCustomerResponse>>> GetAllAsync(
        GetAllCustomerQuery query,
        CancellationToken cancellationToken = default);

    Task<List<GetAllCustomerForDataSourceResponse>> GetAllForDataSourceAsync(
        CancellationToken cancellationToken = default);

    Task<GetCustomerByIdResponse?> GetByIdAsync(
        GetCustomerByIdQuery query,
        CancellationToken cancellationToken = default);

    Task<AddCustomerResponse> AddAsync(
        AddCustomerCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<UpdateCustomerResponse?> UpdateAsync(
        UpdateCustomerCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(DeleteCustomerCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task<CustomerInsightsResponse> GetInsightsAsync(
        GetCustomerInsightsQuery query,
        CancellationToken cancellationToken = default);

    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}