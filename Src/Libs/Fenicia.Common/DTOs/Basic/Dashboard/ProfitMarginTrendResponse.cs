using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Dashboard;

public record ProfitMarginTrendResponse(
    [Required] [MaxLength(200)] string Period,
    [Required] DateTime Date,
    decimal MarginPercentage,
    [Required] [MaxLength(200)] string Trend);