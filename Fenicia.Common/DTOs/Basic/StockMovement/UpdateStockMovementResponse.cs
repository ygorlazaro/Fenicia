using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.DTOs.Basic.StockMovement;

public class UpdateStockMovementResponse()
{
    public UpdateStockMovementResponse(
        Guid id,
        Guid productId,
        double quantity,
        DateTime? date,
        decimal? price,
        EnumStockMovementType type,
        Guid? customerId,
        Guid? supplierId,
        Guid? employeeId,
        Guid? orderId,
        string? reason)
        : this()
    {
        Id = id;
        ProductId = productId;
        Quantity = quantity;
        Date = date;
        Price = price;
        Type = type;
        CustomerId = customerId;
        SupplierId = supplierId;
        EmployeeId = employeeId;
        OrderId = orderId;
        Reason = reason;
    }

    [Required]
    public Guid Id { get; init; }

    [Required]
    public Guid ProductId { get; set; }

    public double Quantity { get; set; }

    public DateTime? Date { get; set; }

    [Required]
    public decimal? Price { get; set; }

    [Required]
    public EnumStockMovementType Type { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? SupplierId { get; set; }

    public Guid? EmployeeId { get; set; }

    public Guid? OrderId { get; set; }

    [MaxLength(255)]
    public string? Reason { get; set; }
}
