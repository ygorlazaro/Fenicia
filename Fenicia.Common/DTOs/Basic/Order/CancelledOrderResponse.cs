using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Order;

public class CancelledOrderResponse()
{
    public CancelledOrderResponse(
        Guid orderId,
        string customerName,
        decimal totalAmount,
        DateTime saleDate,
        int totalItems,
        string? cancelledReason)
        : this()
    {
        OrderId = orderId;
        CustomerName = customerName;
        TotalAmount = totalAmount;
        SaleDate = saleDate;
        TotalItems = totalItems;
        CancelledReason = cancelledReason;
    }

    [Required]
    public Guid OrderId { get; set; }

    [Required]
    [MaxLength(200)]
    public string CustomerName { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    [Required]
    public DateTime SaleDate { get; set; }

    public int TotalItems { get; set; }

    public string? CancelledReason { get; set; }
}
