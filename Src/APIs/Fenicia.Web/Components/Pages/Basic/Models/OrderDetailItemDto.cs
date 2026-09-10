namespace Fenicia.Web.Components.Pages.Basic.Models;

public class OrderDetailItemDto
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public decimal DiscountAmount { get; set; }

    public double Quantity { get; set; }

    public decimal Subtotal { get; set; }
}
