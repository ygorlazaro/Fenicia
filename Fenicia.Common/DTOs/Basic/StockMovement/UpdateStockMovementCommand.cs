using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.DTOs.Basic.StockMovement;

public class UpdateStockMovementRequest()
{
    public UpdateStockMovementRequest(
        Guid id,
        double quantity,
        DateTime? date,
        decimal? price,
        EnumStockMovementType type,
        Guid productId,
        Guid? customerId,
        Guid? supplierId,
        Guid? employeeId,
        Guid? orderId,
        string? reason)
        : this()
    {
        Id = id;
        Quantity = quantity;
        Date = date;
        Price = price;
        Type = type;
        ProductId = productId;
        CustomerId = customerId;
        SupplierId = supplierId;
        EmployeeId = employeeId;
        OrderId = orderId;
        Reason = reason;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public double Quantity { get; set; }

    public DateTime? Date { get; set; }

    [Required]
    public decimal? Price { get; set; }

    [Required]
    public EnumStockMovementType Type { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? SupplierId { get; set; }

    public Guid? EmployeeId { get; set; }

    public Guid? OrderId { get; set; }

    [MaxLength(255)]
    public string? Reason { get; set; }
}
