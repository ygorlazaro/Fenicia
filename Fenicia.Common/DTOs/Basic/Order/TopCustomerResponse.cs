using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Order;

public class TopCustomerResponse()
{
    public TopCustomerResponse(
        Guid customerId,
        string customerName,
        int orderCount,
        decimal totalSpent,
        int totalItems)
        : this()
    {
        CustomerId = customerId;
        CustomerName = customerName;
        OrderCount = orderCount;
        TotalSpent = totalSpent;
        TotalItems = totalItems;
    }

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    [MaxLength(200)]
    public string CustomerName { get; set; } = string.Empty;

    public int OrderCount { get; set; }

    public decimal TotalSpent { get; set; }

    public int TotalItems { get; set; }
}
