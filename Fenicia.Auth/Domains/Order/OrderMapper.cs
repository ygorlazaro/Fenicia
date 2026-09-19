using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Order;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Auth.Domains.Order;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class OrderMapper
{
    [MapProperty(nameof(OrderModel.Id), nameof(OrderResponse.OrderId))]
    public partial OrderResponse MapToOrderResponse(OrderModel order);
}
