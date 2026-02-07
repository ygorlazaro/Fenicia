namespace Fenicia.Common.Data.Requests.Auth;

public class OrderRequest
{
    public IEnumerable<OrderDetailRequest> Details { get; set; } = null!;
}