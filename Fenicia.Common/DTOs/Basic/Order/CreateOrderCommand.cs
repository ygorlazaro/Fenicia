using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.DTOs.Basic.Order;

public class CreateOrderRequest()
{
    public CreateOrderRequest(
        Guid userId,
        Guid customerId,
        DateTime saleDate,
        EnumOrderStatus status,
        List<OrderDetailRequest> details,
        EnumPaymentMethod paymentMethod,
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
    public EnumOrderStatus Status { get; set; }

    public List<OrderDetailRequest> Details { get; set; } = [];

    [Required]
    public EnumPaymentMethod PaymentMethod { get; set; }

    public Guid? EmployeeId { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    [Range(0, double.MaxValue)]
    public decimal DiscountAmount { get; set; }
}
