using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Basic.Domains.OrderDetail;

public interface IOrderDetailRepository : IRepository<OrderDetailModel>
{
    Task<IEnumerable<OrderDetailModel>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, int>> GetDetailCountsByOrderIdsAsync(
        IEnumerable<Guid> orderIds,
        CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, double>> GetQuantitySumsByOrderIdsAsync(
        IEnumerable<Guid> orderIds,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<OrderDetailModel>> GetByOrderDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<OrderDetailModel>> GetByDateRangeAsync(
        DateTime startDate,
        CancellationToken cancellationToken = default);
}