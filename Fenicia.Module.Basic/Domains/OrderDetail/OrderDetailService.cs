using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.OrderDetail;
using Fenicia.Module.Basic.Domains.OrderDetail.Interfaces;

namespace Fenicia.Module.Basic.Domains.OrderDetail;

public sealed class OrderDetailService(IOrderDetailRepository repository) : IOrderDetailService
{
    public OrderDetailService()
        : this(null!)
    {
    }

    public async Task<List<GetOrderDetailsByOrderIdResponse>> GetByOrderIdAsync(
        GetOrderDetailsByOrderIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var details = await repository.GetByOrderIdAsync(query.OrderId, cancellationToken);

        return [.. details.Select(d => new GetOrderDetailsByOrderIdResponse(
            d.Id,
            d.OrderId,
            d.ProductId,
            d.Product.Name,
            d.Price,
            d.DiscountAmount,
            d.Quantity,
            d.Subtotal))];
    }

    public Task<Dictionary<Guid, int>> GetDetailCountsByOrderIdsAsync(
        IEnumerable<Guid> orderIds,
        CancellationToken cancellationToken = default)
    {
        return repository.GetDetailCountsByOrderIdsAsync(orderIds, cancellationToken);
    }

    public Task<Dictionary<Guid, double>> GetQuantitySumsByOrderIdsAsync(
        IEnumerable<Guid> orderIds,
        CancellationToken cancellationToken = default)
    {
        return repository.GetQuantitySumsByOrderIdsAsync(orderIds, cancellationToken);
    }

    public async Task<List<OrderDetailModel>> GetByOrderDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var result = await repository.GetByOrderDateRangeAsync(startDate, endDate, cancellationToken);
        return [.. result];
    }

    public async Task<List<OrderDetailModel>> GetByDateRangeAsync(
        DateTime startDate,
        CancellationToken cancellationToken = default)
    {
        var result = await repository.GetByDateRangeAsync(startDate, cancellationToken);
        return [.. result];
    }
}