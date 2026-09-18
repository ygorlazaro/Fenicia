using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.StockMovement;

public class StockMovementHistoryResponse()
{
    public StockMovementHistoryResponse(
        Guid id,
        Guid productId,
        string productName,
        double quantity,
        DateTime date,
        decimal price,
        string type,
        string? reason,
        string? customerName,
        string? supplierName)
        : this()
    {
        Id = id;
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        Date = date;
        Price = price;
        Type = type;
        Reason = reason;
        CustomerName = customerName;
        SupplierName = supplierName;
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

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    [MaxLength(200)]
    public string Type { get; set; } = string.Empty;

    public string? Reason { get; set; }

    [MaxLength(50)]
    public string? CustomerName { get; set; }

    [MaxLength(50)]
    public string? SupplierName { get; set; }
}
