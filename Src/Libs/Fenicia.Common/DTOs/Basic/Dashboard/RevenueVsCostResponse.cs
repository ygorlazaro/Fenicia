using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Dashboard;

public record RevenueVsCostResponse(
    [Required] [MaxLength(200)] string Period,
    [Required] DateTime Date,
    decimal Revenue,
    decimal Cost,
    decimal Profit);