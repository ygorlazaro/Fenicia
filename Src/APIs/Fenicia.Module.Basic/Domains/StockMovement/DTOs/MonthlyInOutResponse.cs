using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.StockMovement.DTOs;

public record MonthlyInOutResponse(
    [Required] [MaxLength(200)] string Month,
    double TotalIn,
    double TotalOut,
    decimal TotalInValue,
    decimal TotalOutValue);