using System.ComponentModel.DataAnnotations;

using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.Data.Requests.Basic;

public class OrderRequest
{
    public Guid UserId { get; set; }

    [Required]
    public Guid CustomerId { get; set; }

    public List<OrderDetailRequest> Details { get; set; }

    public DateTime SaleDate { get; set; }

    public OrderStatus Status { get; set; }

    public Guid Id { get; set; }
}