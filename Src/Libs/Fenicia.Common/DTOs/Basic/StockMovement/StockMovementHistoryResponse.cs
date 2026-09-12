using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.StockMovement;

public record StockMovementHistoryResponse(
    [Required] Guid Id,
    [Required] Guid ProductId,
    [Required] [MaxLength(200)] string ProductName,
    double Quantity,
    [Required] DateTime Date,
    decimal Price,
    [Required] [MaxLength(200)] string Type,
    string? Reason,
    string? CustomerName,
    string? SupplierName);