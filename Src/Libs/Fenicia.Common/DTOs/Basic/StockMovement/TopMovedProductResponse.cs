using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.StockMovement;

public record TopMovedProductResponse(
    [Required] Guid ProductId,
    [Required] [MaxLength(200)] string ProductName,
    [Required] [MaxLength(200)] string CategoryName,
    double TotalMoved,
    decimal TotalValue,
    int MovementCount);