namespace Fenicia.Web.Components.Pages.Basic.Models;

public class OrderFormCartItem
{
    public Guid ProductId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public double Quantity { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal Subtotal { get; set; }
}
