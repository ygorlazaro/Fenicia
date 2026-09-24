using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.DTOs.Basic.StockMovement;

public class GetStockMovementResponse()
{
    public GetStockMovementResponse(
        Guid id,
        Guid productId,
        string productName,
        double quantity,
        DateTime? date,
        decimal? price,
        EnumStockMovementType type,
        Guid? customerId,
        string? customerName,
        Guid? supplierId,
        string? supplierName,
        Guid? employeeId,
        string? employeeName,
        Guid? orderId,
        string? reason)
        : this()
    {
        Id = id;
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        Date = date;
        Price = price;
        Type = type;
        CustomerId = customerId;
        CustomerName = customerName;
        SupplierId = supplierId;
        SupplierName = supplierName;
        EmployeeId = employeeId;
        EmployeeName = employeeName;
        OrderId = orderId;
        Reason = reason;
    }

    [Required]
    public Guid Id { get; init; }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [MaxLength(50)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    public double Quantity { get; set; }

    public DateTime? Date { get; set; }

    [Required]
    public decimal? Price { get; set; }

    [Required]
    public EnumStockMovementType Type { get; set; }

    public Guid? CustomerId { get; set; }

    [MaxLength(50)]
    public string? CustomerName { get; set; }

    public Guid? SupplierId { get; set; }

    [MaxLength(50)]
    public string? SupplierName { get; set; }

    public Guid? EmployeeId { get; set; }

    [MaxLength(50)]
    public string? EmployeeName { get; set; }

    public Guid? OrderId { get; set; }

    [MaxLength(255)]
    public string? Reason { get; set; }
}
