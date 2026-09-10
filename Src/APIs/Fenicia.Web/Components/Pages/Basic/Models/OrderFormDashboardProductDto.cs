namespace Fenicia.Web.Components.Pages.Basic.Models;

public class OrderFormDashboardProductDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? SKU { get; set; }

    public string? Barcode { get; set; }

    public decimal? CostPrice { get; set; }

    public decimal SalesPrice { get; set; }

    public double Quantity { get; set; }

    public string? UnitOfMeasure { get; set; }

    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
