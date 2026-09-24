using Fenicia.Common.DTOs.Basic.Order;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.OrderDetail;

/// <summary>
/// Mapper class for mapping order detail data.
/// </summary>
public static class OrderDetailMapper
{
    /// <summary>
    /// Maps an OrderDetailModel to an OrderDetailResponse.
    /// </summary>
    /// <param name="detail">The order detail model.</param>
    /// <returns>The order detail response.</returns>
    public static OrderDetailResponse MapToResponse(OrderDetailModel detail)
    {
        return new OrderDetailResponse(
            detail.Id,
            detail.ProductId,
            detail.Product.Name,
            detail.Price,
            detail.DiscountAmount,
            detail.Quantity,
            detail.Subtotal);
    }

    /// <summary>
    /// Maps a collection of OrderDetailModel to a list of OrderDetailResponse.
    /// </summary>
    /// <param name="details">The order detail models.</param>
    /// <returns>A list of order detail responses.</returns>
    public static List<OrderDetailResponse> MapToResponseList(IEnumerable<OrderDetailModel> details)
    {
        return [.. details.Select(MapToResponse)];
    }
}