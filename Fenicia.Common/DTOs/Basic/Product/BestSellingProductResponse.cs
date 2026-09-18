using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Product;

public class BestSellingProductResponse
{
    public BestSellingProductResponse()
    {
    }

    public BestSellingProductResponse(
        Guid productId,
        string productName,
        string categoryName,
        double totalQuantitySold,
        decimal totalRevenue,
        int orderCount,
        decimal averagePrice)
    {
        ProductId = productId;
        ProductName = productName;
        CategoryName = categoryName;
        TotalQuantitySold = totalQuantitySold;
        TotalRevenue = totalRevenue;
        OrderCount = orderCount;
        AveragePrice = averagePrice;
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

    public decimal AveragePrice { get; set; }
}
