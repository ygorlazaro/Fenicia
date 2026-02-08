using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.Data.Responses.Basic;

public class OrderResponse
{
    public OrderResponse(OrderModel model)
    {
        this.CustomerId = model.CustomerId;
        this.Details = [.. model.Details.Select(d => new OrderDetailResponse(d))];
        this.SaleDate = model.SaleDate;
        this.Status = model.Status;
        this.TotalAmount = model.TotalAmount;
        this.UserId = model.UserId;
        this.Id = model.Id;
    }

    public Guid CustomerId { get; set; }

    public List<OrderDetailResponse> Details { get; set; }

    public DateTime SaleDate { get; set; }

    public OrderStatus Status { get; set; }

    public decimal TotalAmount { get; set; }

    public Guid UserId { get; set; }

    public Guid Id { get; set; }
}
