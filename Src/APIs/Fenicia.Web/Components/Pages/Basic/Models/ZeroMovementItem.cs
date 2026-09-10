namespace Fenicia.Web.Components.Pages.Basic.Models;

public class ZeroMovementItem
{
    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string? SupplierName { get; set; }

    public double CurrentStock { get; set; }

    public decimal StockValue { get; set; }

    public DateTime? LastMovementDate { get; set; }

    public int DaysWithoutMovement { get; set; }
}
