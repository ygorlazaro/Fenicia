using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.Database.Requests;

public class OrderRequest
{
    [Required(ErrorMessage = "Order details are required")]
    [MinLength(length: 1, ErrorMessage = "At least one order detail is required")]
    public IEnumerable<OrderDetailRequest> Details { get; set; } = null!;
}
