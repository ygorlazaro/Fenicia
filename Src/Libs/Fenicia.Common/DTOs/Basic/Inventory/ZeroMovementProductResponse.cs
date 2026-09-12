using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Inventory;

public record ZeroMovementProductResponse(
    [Required] Guid ProductId,
    [Required] [MaxLength(200)] string ProductName,
    [Required] Guid CategoryId,
    [Required] [MaxLength(200)] string CategoryName,
    string? SupplierName,
    double CurrentStock,
    decimal StockValue,
    DateTime? LastMovementDate,
    int DaysWithoutMovement);