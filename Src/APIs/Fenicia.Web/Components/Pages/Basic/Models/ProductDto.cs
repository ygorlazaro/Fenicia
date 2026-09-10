using Fenicia.Web.Services;

namespace Fenicia.Web.Components.Pages.Basic.Models;

public class ProductDto : IEquatable<ProductDto>, ICrudItem
{
    public Guid Id { get; init; }

    public string Name { get; set; } = string.Empty;

    public string? SKU { get; set; }

    public string? Barcode { get; set; }

    public string? Description { get; set; }

    public decimal? CostPrice { get; set; }

    public decimal SalesPrice { get; set; }

    public double Quantity { get; set; }

    public int? MinStockLevel { get; set; }

    public int? MaxStockLevel { get; set; }

    public string? ImageUrl { get; set; }

    public decimal? Weight { get; set; }

    public string? Dimensions { get; set; }

    public string? UnitOfMeasure { get; set; }

    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public Guid? SupplierId { get; set; }

    public string? SupplierName { get; set; }

    public bool IsActive { get; set; }

    public bool Equals(ProductDto? other)
    {
        if (other is null)
        {
            return false;
        }

        return Id == other.Id && Name == other.Name;
    }

    public override bool Equals(object? obj) => Equals(obj as ProductDto);

    public override int GetHashCode() => HashCode.Combine(Id, Name);
}
