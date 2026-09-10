namespace Fenicia.Web.Components.Pages.Basic.Models;

public class OrderFormRecentOrderDto
{
    public Guid Id { get; set; }

    public string OrderNumber { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public int TotalQuantity { get; set; }

    public int TotalItems { get; set; }

    public DateTime SaleDate { get; set; }

    public string Status { get; set; } = string.Empty;
}
