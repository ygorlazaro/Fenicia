using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Customer;

public class CustomerRecentOrdersResponse()
{
    public CustomerRecentOrdersResponse(Guid orderId, Guid customerId, string customerName, decimal totalAmount, DateTime saleDate, string status, int totalItems)
        : this()
    {
        OrderId = orderId;
        CustomerId = customerId;
        CustomerName = customerName;
        TotalAmount = totalAmount;
        SaleDate = saleDate;
        Status = status;
        TotalItems = totalItems;
    }

    [Required]
    public Guid OrderId { get; set; }

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    [MaxLength(50)]
    public string CustomerName { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    [Required]
    public DateTime SaleDate { get; set; }

    [Required]
    [MaxLength(200)]
    public string Status { get; set; } = string.Empty;

    public int TotalItems { get; set; }
}
