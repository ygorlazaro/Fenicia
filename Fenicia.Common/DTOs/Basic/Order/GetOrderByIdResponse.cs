using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.DTOs.Basic.Order;

public class GetOrderByIdResponse() : ICrudItem
{
    public GetOrderByIdResponse(
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
        PaymentMethod paymentMethod,
        string? notes,
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
        Notes = notes;
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
    public PaymentMethod PaymentMethod { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public Guid? EmployeeId { get; set; }

    public string? EmployeeName { get; set; }
}
