using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.OrderDetail.DTOs;

namespace Fenicia.Module.Basic.Domains.OrderDetail.Interfaces;

public interface IOrderDetailService
{
    Task<List<GetOrderDetailsByOrderIdResponse>> GetByOrderIdAsync(
        GetOrderDetailsByOrderIdQuery query,
        CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, int>> GetDetailCountsByOrderIdsAsync(
        IEnumerable<Guid> orderIds,
        CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, double>> GetQuantitySumsByOrderIdsAsync(
        IEnumerable<Guid> orderIds,
        CancellationToken cancellationToken = default);

    Task<List<OrderDetailModel>> GetByOrderDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    Task<List<OrderDetailModel>> GetByDateRangeAsync(DateTime startDate, CancellationToken cancellationToken = default);
}