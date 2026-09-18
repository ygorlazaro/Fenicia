using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.StockMovement;

public class TopMovedProductResponse()
{
    public TopMovedProductResponse(
        Guid productId,
        string productName,
        string categoryName,
        double totalMoved,
        decimal totalValue,
        int movementCount)
        : this()
    {
        ProductId = productId;
        ProductName = productName;
        CategoryName = categoryName;
        TotalMoved = totalMoved;
        TotalValue = totalValue;
        MovementCount = movementCount;
    }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [MaxLength(50)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string CategoryName { get; set; } = string.Empty;

    public double TotalMoved { get; set; }

    public decimal TotalValue { get; set; }

    public int MovementCount { get; set; }
}
