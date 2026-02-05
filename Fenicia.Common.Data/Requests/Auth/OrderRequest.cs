using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.Data.Requests.Auth;

public class OrderRequest
{
    [Required(ErrorMessage = "Order details are required")]
    [MinLength(1, ErrorMessage = "At least one order detail is required")]
    public IEnumerable<OrderDetailRequest> Details { get; set; } = null!;
}
