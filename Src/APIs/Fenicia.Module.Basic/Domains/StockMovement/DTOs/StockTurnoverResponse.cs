using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.StockMovement.DTOs;

public record StockTurnoverResponse(
    [Required] Guid ProductId,
    [Required] [MaxLength(200)] string ProductName,
    [Required] [MaxLength(200)] string CategoryName,
    double CurrentStock,
    double TotalSold,
    double TurnoverRate,
    [Required] [MaxLength(200)] string TurnoverClassification);