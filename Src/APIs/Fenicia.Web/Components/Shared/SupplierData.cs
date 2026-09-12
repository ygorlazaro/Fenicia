namespace Fenicia.Web.Components.Shared;

public class SupplierData
{
    public Guid SupplierId { get; set; }

    public string SupplierName { get; set; } = string.Empty;

    public decimal TotalCostValue { get; set; }

    public decimal TotalSalesValue { get; set; }

    public double TotalQuantity { get; set; }
}
