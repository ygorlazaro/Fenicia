using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Dashboard;

public class RevenueVsCostResponse()
{
    public RevenueVsCostResponse(string period, DateTime date, decimal revenue, decimal cost, decimal profit)
        : this()
    {
        Period = period;
        Date = date;
        Revenue = revenue;
        Cost = cost;
        Profit = profit;
    }

    [Required]
    [MaxLength(200)]
    public string Period { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    public decimal Revenue { get; set; }

    public decimal Cost { get; set; }

    public decimal Profit { get; set; }
}
