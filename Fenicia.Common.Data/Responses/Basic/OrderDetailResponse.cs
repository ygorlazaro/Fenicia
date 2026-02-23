using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Common.Data.Responses.Basic;

public class OrderDetailResponse(OrderDetailModel model)
{
    public Guid ProductId { get; set; } = model.ProductId;

    public decimal Price { get; set; } = model.Price;

    public Guid OrderId { get; set; } = model.OrderId;

    public Guid Id { get; set; } = model.Id;

    public double Quantity { get; set; } = model.Quantity;
}