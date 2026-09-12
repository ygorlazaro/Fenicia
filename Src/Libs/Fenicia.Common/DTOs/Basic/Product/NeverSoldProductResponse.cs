using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Product;

public record NeverSoldProductResponse(
    [Required] Guid ProductId,
    [Required] [MaxLength(200)] string ProductName,
    [Required] [MaxLength(200)] string CategoryName,
    string? SupplierName,
    double CurrentStock,
    decimal CostValue,
    DateTime? LastStockMovement);