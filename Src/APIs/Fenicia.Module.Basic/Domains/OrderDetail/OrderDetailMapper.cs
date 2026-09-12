using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.OrderDetail;
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
            detail.Product.Name,
            detail.Price,
            detail.DiscountAmount,
            detail.Quantity,
            detail.Subtotal);
    }
}