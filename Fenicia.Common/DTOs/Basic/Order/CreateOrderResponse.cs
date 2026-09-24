using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.DTOs.Basic.Order;

public class CreateOrderResponse()
{
    public CreateOrderResponse(
        Guid id,
        string orderNumber,
        Guid userId,
        Guid customerId,
        decimal totalAmount,
        decimal discountAmount,
        int totalQuantity,
        DateTime saleDate,
        EnumOrderStatus status,
        EnumPaymentMethod paymentMethod,
        string? notes = null,
        Guid? employeeId = null)
        : this()
    {
        Id = id;
        OrderNumber = orderNumber;
        UserId = userId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
        DiscountAmount = discountAmount;
        TotalQuantity = totalQuantity;
        SaleDate = saleDate;
        Status = status;
        PaymentMethod = paymentMethod;
        Notes = notes;
        EmployeeId = employeeId;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(40)]
    public string OrderNumber { get; set; } = string.Empty;

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid CustomerId { get; set; }

    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal DiscountAmount { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalQuantity { get; set; }

    [Required]
    public DateTime SaleDate { get; set; }

    [Required]
    public EnumOrderStatus Status { get; set; }

    [Required]
    public EnumPaymentMethod PaymentMethod { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public Guid? EmployeeId { get; set; }
}
