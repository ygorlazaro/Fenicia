using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Order;

public class OrderStatusCountResponse()
{
    public OrderStatusCountResponse(
        string status,
        int count,
        decimal totalValue)
        : this()
    {
        Status = status;
        Count = count;
        TotalValue = totalValue;
    }

    [Required]
    [MaxLength(200)]
    public string Status { get; set; } = string.Empty;

    public int Count { get; set; }

    [Range(0, double.MaxValue)]
    public decimal TotalValue { get; set; }
}
