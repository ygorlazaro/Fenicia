using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Dashboard;

public class ProfitMarginTrendResponse()
{
    public ProfitMarginTrendResponse(string period, DateTime date, decimal marginPercentage, string trend)
        : this()
    {
        Period = period;
        Date = date;
        MarginPercentage = marginPercentage;
        Trend = trend;
    }

    [Required]
    [MaxLength(200)]
    public string Period { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    public decimal MarginPercentage { get; set; }

    [Required]
    [MaxLength(200)]
    public string Trend { get; set; } = string.Empty;
}
