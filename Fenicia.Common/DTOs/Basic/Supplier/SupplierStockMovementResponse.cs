using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Supplier;

public class SupplierStockMovementResponse()
{
    public SupplierStockMovementResponse(
        Guid movementId,
        Guid productId,
        string productName,
        double quantity,
        decimal price,
        DateTime date,
        string movementType)
        : this()
    {
        MovementId = movementId;
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        Price = price;
        Date = date;
        MovementType = movementType;
    }

    [Required]
    public Guid MovementId { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [MaxLength(50)]
    public string ProductName { get; set; } = string.Empty;

    public double Quantity { get; set; }

    public decimal Price { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [MaxLength(200)]
    public string MovementType { get; set; } = string.Empty;
}
