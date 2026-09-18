using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.DTOs.Basic.Order;

public class CreateOrderCommand()
{
    public CreateOrderCommand(
        Guid userId,
        Guid customerId,
        DateTime saleDate,
        OrderStatus status,
        List<OrderDetailCommand> details,
        PaymentMethod paymentMethod,
        Guid? employeeId = null,
        string? notes = null,
        decimal discountAmount = 0)
        : this()
    {
        UserId = userId;
        CustomerId = customerId;
        SaleDate = saleDate;
        Status = status;
        Details = details;
        PaymentMethod = paymentMethod;
        EmployeeId = employeeId;
        Notes = notes;
        DiscountAmount = discountAmount;
    }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    public DateTime SaleDate { get; set; }

    [Required]
    public OrderStatus Status { get; set; }

    public List<OrderDetailCommand> Details { get; set; } = [];

    [Required]
    public PaymentMethod PaymentMethod { get; set; }

    public Guid? EmployeeId { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    [Range(0, double.MaxValue)]
    public decimal DiscountAmount { get; set; }
}
