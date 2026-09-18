using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Inventory;

public class ZeroMovementProductResponse()
{
    public ZeroMovementProductResponse(
        Guid productId,
        string productName,
        Guid categoryId,
        string categoryName,
        string? supplierName,
        double currentStock,
        decimal stockValue,
        DateTime? lastMovementDate,
        int daysWithoutMovement)
        : this()
    {
        ProductId = productId;
        ProductName = productName;
        CategoryId = categoryId;
        CategoryName = categoryName;
        SupplierName = supplierName;
        CurrentStock = currentStock;
        StockValue = stockValue;
        LastMovementDate = lastMovementDate;
        DaysWithoutMovement = daysWithoutMovement;
    }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    public Guid CategoryId { get; set; }

    [Required]
    [MaxLength(200)]
    public string CategoryName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? SupplierName { get; set; }

    public double CurrentStock { get; set; }

    public decimal StockValue { get; set; }

    public DateTime? LastMovementDate { get; set; }

    public int DaysWithoutMovement { get; set; }
}
