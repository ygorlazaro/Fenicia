using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Dashboard.DTOs;

public record ProfitMarginTrendResponse(
    [Required] [MaxLength(200)] string Period,
    [Required] DateTime Date,
    decimal MarginPercentage,
    [Required] [MaxLength(200)] string Trend);