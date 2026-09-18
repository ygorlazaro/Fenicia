using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Product;

public class NeverSoldProductResponse
{
    public NeverSoldProductResponse()
    {
    }

    public NeverSoldProductResponse(
        Guid productId,
        string productName,
        string categoryName,
        string? supplierName,
        double currentStock,
        decimal costValue,
        DateTime? lastStockMovement)
    {
        ProductId = productId;
        ProductName = productName;
        CategoryName = categoryName;
        SupplierName = supplierName;
        CurrentStock = currentStock;
        CostValue = costValue;
        LastStockMovement = lastStockMovement;
    }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string CategoryName { get; set; } = string.Empty;

    public string? SupplierName { get; set; }

    public double CurrentStock { get; set; }

    public decimal CostValue { get; set; }

    public DateTime? LastStockMovement { get; set; }
}
