namespace Fenicia.Auth.Domains.Order.Data;

using System.ComponentModel.DataAnnotations;

using OrderDetail.Data;

public class OrderRequest
{
    [Required(ErrorMessage = "Order details are required")]
    [MinLength(length: 1, ErrorMessage = "At least one order detail is required")]
    public IEnumerable<OrderDetailRequest> Details { get; set; } = null!;
}
