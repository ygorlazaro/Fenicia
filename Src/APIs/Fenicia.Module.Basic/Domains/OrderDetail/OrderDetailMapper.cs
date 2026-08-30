using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.OrderDetail.DTOs;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.OrderDetail;

[Mapper]
public static partial class OrderDetailMapper
{
    public static GetOrderDetailsByOrderIdResponse MapToGetOrderDetailsByOrderIdResponse(this OrderDetailModel detail)
    {
        return new GetOrderDetailsByOrderIdResponse(
            detail.Id,
            detail.OrderId,
            detail.ProductId,
            detail.Product?.Name ?? string.Empty,
            detail.Price,
            detail.DiscountAmount,
            detail.Quantity,
            detail.Subtotal);
    }
}
