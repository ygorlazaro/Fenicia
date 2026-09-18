using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Inventory;

public class StockValueByCategoryResponse()
{
    public StockValueByCategoryResponse(
        Guid categoryId,
        string categoryName,
        int productCount,
        decimal totalStockValue,
        double percentage)
        : this()
    {
        CategoryId = categoryId;
        CategoryName = categoryName;
        ProductCount = productCount;
        TotalStockValue = totalStockValue;
        Percentage = percentage;
    }

    [Required]
    public Guid CategoryId { get; set; }

    [Required]
    [MaxLength(200)]
    public string CategoryName { get; set; } = string.Empty;

    public int ProductCount { get; set; }

    public decimal TotalStockValue { get; set; }

    public double Percentage { get; set; }
}
