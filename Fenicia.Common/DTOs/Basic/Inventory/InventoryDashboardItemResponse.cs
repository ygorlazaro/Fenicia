using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Inventory;

public class InventoryDashboardItemResponse()
{
    public InventoryDashboardItemResponse(
        Guid id,
        string name,
        double quantity,
        decimal? costPrice,
        decimal salesPrice,
        Guid categoryId,
        string categoryName)
        : this()
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        CostPrice = costPrice;
        SalesPrice = salesPrice;
        CategoryId = categoryId;
        CategoryName = categoryName;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public double Quantity { get; set; }

    public decimal? CostPrice { get; set; }

    [Required]
    public decimal SalesPrice { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [Required]
    [MaxLength(200)]
    public string CategoryName { get; set; } = string.Empty;
}
