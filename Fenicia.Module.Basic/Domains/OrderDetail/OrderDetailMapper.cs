using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.OrderDetail;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.OrderDetail;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class OrderDetailMapper
{
    public GetOrderDetailsByOrderIdResponse MapToGetOrderDetailsByOrderIdResponse(OrderDetailModel detail)
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
