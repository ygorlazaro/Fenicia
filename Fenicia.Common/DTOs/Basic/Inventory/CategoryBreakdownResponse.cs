using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Inventory;

public class CategoryBreakdownResponse()
{
    public CategoryBreakdownResponse(
        Guid categoryId,
        string categoryName,
        decimal totalCostValue,
        decimal totalSalesValue,
        double totalQuantity)
        : this()
    {
        CategoryId = categoryId;
        CategoryName = categoryName;
        TotalCostValue = totalCostValue;
        TotalSalesValue = totalSalesValue;
        TotalQuantity = totalQuantity;
    }

    [Required]
    public Guid CategoryId { get; set; }

    [Required]
    [MaxLength(200)]
    public string CategoryName { get; set; } = string.Empty;

    public decimal TotalCostValue { get; set; }

    public decimal TotalSalesValue { get; set; }

    public double TotalQuantity { get; set; }
}
