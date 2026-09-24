using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.DTOs.Basic.Order;

public class GetAllOrderResponse() : ICrudItem
{
    public GetAllOrderResponse(
        Guid id,
        string orderNumber,
        Guid userId,
        Guid customerId,
        string customerName,
        decimal totalAmount,
        decimal discountAmount,
        int totalQuantity,
        DateTime saleDate,
        string status,
        EnumPaymentMethod paymentMethod,
        int totalItems,
        Guid? employeeId = null,
        string? employeeName = null)
        : this()
    {
        Id = id;
        OrderNumber = orderNumber;
        UserId = userId;
        CustomerId = customerId;
        CustomerName = customerName;
        TotalAmount = totalAmount;
        DiscountAmount = discountAmount;
        TotalQuantity = totalQuantity;
        SaleDate = saleDate;
        Status = status;
        PaymentMethod = paymentMethod;
        TotalItems = totalItems;
        EmployeeId = employeeId;
        EmployeeName = employeeName;
    }

    [Required]
    public Guid Id { get; init; }

    [Required]
    [MaxLength(40)]
    public string OrderNumber { get; set; } = string.Empty;

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    [MaxLength(200)]
    public string CustomerName { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal DiscountAmount { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalQuantity { get; set; }

    [Required]
    public DateTime SaleDate { get; set; }

    [Required]
    [MaxLength(200)]
    public string Status { get; set; } = string.Empty;

    [Required]
    public EnumPaymentMethod PaymentMethod { get; set; }

    public int TotalItems { get; set; }

    public Guid? EmployeeId { get; set; }

    public string? EmployeeName { get; set; }
}
