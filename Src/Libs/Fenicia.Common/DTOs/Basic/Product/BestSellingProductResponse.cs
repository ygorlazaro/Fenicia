using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Product;

public record BestSellingProductResponse(
    [Required] Guid ProductId,
    [Required] [MaxLength(200)] string ProductName,
    [Required] [MaxLength(200)] string CategoryName,
    double TotalQuantitySold,
    decimal TotalRevenue,
    int OrderCount,
    decimal AveragePrice);