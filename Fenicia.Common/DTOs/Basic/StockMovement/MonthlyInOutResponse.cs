using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.StockMovement;

public class MonthlyInOutResponse()
{
    public MonthlyInOutResponse(
        string month,
        double totalIn,
        double totalOut,
        decimal totalInValue,
        decimal totalOutValue)
        : this()
    {
        Month = month;
        TotalIn = totalIn;
        TotalOut = totalOut;
        TotalInValue = totalInValue;
        TotalOutValue = totalOutValue;
    }

    [Required]
    [MaxLength(200)]
    public string Month { get; set; } = string.Empty;

    public double TotalIn { get; set; }

    public double TotalOut { get; set; }

    public decimal TotalInValue { get; set; }

    public decimal TotalOutValue { get; set; }
}
