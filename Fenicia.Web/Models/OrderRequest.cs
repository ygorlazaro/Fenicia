namespace Fenicia.Web.Models;

public class OrderRequest
{
    public IEnumerable<OrderDetailRequest> Details { get; set; } = new List<OrderDetailRequest>();
}
