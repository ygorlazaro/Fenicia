using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Product;

public class WorstSellingProductResponse
{
    public WorstSellingProductResponse()
    {
    }

    public WorstSellingProductResponse(
        Guid productId,
        string productName,
        string categoryName,
        double totalQuantitySold,
        decimal totalRevenue,
        int orderCount,
        double currentStock,
        decimal costValue)
    {
        ProductId = productId;
        ProductName = productName;
        CategoryName = categoryName;
        TotalQuantitySold = totalQuantitySold;
        TotalRevenue = totalRevenue;
        OrderCount = orderCount;
        CurrentStock = currentStock;
        CostValue = costValue;
    }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string CategoryName { get; set; } = string.Empty;

    public double TotalQuantitySold { get; set; }

    public decimal TotalRevenue { get; set; }

    public int OrderCount { get; set; }

    public double CurrentStock { get; set; }

    public decimal CostValue { get; set; }
}
