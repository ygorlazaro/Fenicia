using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Dashboard;

/// <summary>
/// Response DTO containing revenue vs cost comparison data.
/// </summary>
public class RevenueVsCostResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RevenueVsCostResponse"/> class with the specified details.
    /// </summary>
    /// <param name="period">The period label.</param>
    /// <param name="date">The date of the period.</param>
    /// <param name="revenue">The revenue amount.</param>
    /// <param name="cost">The cost amount.</param>
    /// <param name="profit">The profit amount.</param>
    public RevenueVsCostResponse(string period, DateTime date, decimal revenue, decimal cost, decimal profit)
        : this()
    {
        Period = period;
        Date = date;
        Revenue = revenue;
        Cost = cost;
        Profit = profit;
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
    /// Gets or sets the revenue amount.
    /// </summary>
    public decimal Revenue { get; set; }

    /// <summary>
    /// Gets or sets the cost amount.
    /// </summary>
    public decimal Cost { get; set; }

    /// <summary>
    /// Gets or sets the profit amount.
    /// </summary>
    public decimal Profit { get; set; }
}