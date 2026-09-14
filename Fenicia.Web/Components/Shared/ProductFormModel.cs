namespace Fenicia.Web.Components.Shared;

public class ProductFormModel
{
    public string? Name { get; set; }

    public string? SKU { get; set; }

    public string? Barcode { get; set; }

    public decimal? CostPrice { get; set; }

    public decimal SalesPrice { get; set; }

    public double Quantity { get; set; }

    public int? MinStockLevel { get; set; }

    public int? MaxStockLevel { get; set; }

    public string? ImageUrl { get; set; }

    public decimal? Weight { get; set; }

    public string? Dimensions { get; set; }

    public string? UnitOfMeasure { get; set; }

    public string? Description { get; set; }

    public Guid CategoryId { get; set; }

    public Guid? SupplierId { get; set; }
}
