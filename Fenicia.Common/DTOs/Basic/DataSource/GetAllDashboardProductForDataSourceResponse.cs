using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.DataSource;

public class GetAllDashboardProductForDataSourceResponse()
{
    public GetAllDashboardProductForDataSourceResponse(
        Guid id,
        string name,
        string? sku,
        string? barcode,
        decimal? costPrice,
        decimal salesPrice,
        double quantity,
        string? unitOfMeasure,
        Guid categoryId,
        string categoryName,
        bool isActive)
        : this()
    {
        Id = id;
        Name = name;
        SKU = sku;
        Barcode = barcode;
        CostPrice = costPrice;
        SalesPrice = salesPrice;
        Quantity = quantity;
        UnitOfMeasure = unitOfMeasure;
        CategoryId = categoryId;
        CategoryName = categoryName;
        IsActive = isActive;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? SKU { get; set; }

    [MaxLength(50)]
    public string? Barcode { get; set; }

    public decimal? CostPrice { get; set; }

    [Required]
    public decimal SalesPrice { get; set; }

    [Required]
    public double Quantity { get; set; }

    [MaxLength(20)]
    public string? UnitOfMeasure { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [Required]
    [MaxLength(200)]
    public string CategoryName { get; set; } = string.Empty;

    [Required]
    public bool IsActive { get; set; }
}
