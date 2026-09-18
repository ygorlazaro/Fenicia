using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Customer;

public class CustomerOrderHistoryResponse()
{
    public CustomerOrderHistoryResponse(Guid customerId, string customerName, int orderCount, decimal totalSpent, int totalItems, DateTime firstOrderDate, DateTime lastOrderDate, decimal averageOrderValue)
        : this()
    {
        CustomerId = customerId;
        CustomerName = customerName;
        OrderCount = orderCount;
        TotalSpent = totalSpent;
        TotalItems = totalItems;
        FirstOrderDate = firstOrderDate;
        LastOrderDate = lastOrderDate;
        AverageOrderValue = averageOrderValue;
    }

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    [MaxLength(50)]
    public string CustomerName { get; set; } = string.Empty;

    public int OrderCount { get; set; }

    public decimal TotalSpent { get; set; }

    public int TotalItems { get; set; }

    [Required]
    public DateTime FirstOrderDate { get; set; }

    [Required]
    public DateTime LastOrderDate { get; set; }

    public decimal AverageOrderValue { get; set; }
}
