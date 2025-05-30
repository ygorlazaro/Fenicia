using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Requests;

public class NewOrderRequest
{
    [Required]
    public List<NewOrderDetailRequest> Details { get; set; }
}