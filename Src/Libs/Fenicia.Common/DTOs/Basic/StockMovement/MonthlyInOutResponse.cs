using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.StockMovement;

public record MonthlyInOutResponse(
    [Required] [MaxLength(200)] string Month,
    double TotalIn,
    double TotalOut,
    decimal TotalInValue,
    decimal TotalOutValue);