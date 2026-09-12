namespace Fenicia.Web.Components.Shared;

public class CategoryData
{
    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public decimal TotalCostValue { get; set; }

    public decimal TotalSalesValue { get; set; }

    public double TotalQuantity { get; set; }
}
