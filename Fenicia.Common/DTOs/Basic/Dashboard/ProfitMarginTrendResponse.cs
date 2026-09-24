using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Dashboard;

/// <summary>
/// Response DTO containing profit margin trend data.
/// </summary>
public class ProfitMarginTrendResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProfitMarginTrendResponse"/> class with the specified details.
    /// </summary>
    /// <param name="period">The period label.</param>
    /// <param name="date">The date of the period.</param>
    /// <param name="marginPercentage">The margin percentage.</param>
    /// <param name="trend">The trend direction.</param>
    public ProfitMarginTrendResponse(string period, DateTime date, decimal marginPercentage, string trend)
        : this()
    {
        Period = period;
        Date = date;
        MarginPercentage = marginPercentage;
        Trend = trend;
    }

    /// <summary>
    /// Gets or sets the period label.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Period { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date of the period.
    /// </summary>
    [Required]
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the margin percentage.
    /// </summary>
    public decimal MarginPercentage { get; set; }

    /// <summary>
    /// Gets or sets the trend direction.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Trend { get; set; } = string.Empty;
}