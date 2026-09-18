using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Order;

public class SalesTrendResponse()
{
    public SalesTrendResponse(
        string period,
        DateTime date,
        int orderCount,
        decimal totalValue,
        int totalItems)
        : this()
    {
        Period = period;
        Date = date;
        OrderCount = orderCount;
        TotalValue = totalValue;
        TotalItems = totalItems;
    }

    [Required]
    [MaxLength(200)]
    public string Period { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    public int OrderCount { get; set; }

    public decimal TotalValue { get; set; }

    public int TotalItems { get; set; }
}
