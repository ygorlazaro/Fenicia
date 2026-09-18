using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.StockMovement;

public class StockTurnoverResponse()
{
    public StockTurnoverResponse(
        Guid productId,
        string productName,
        string categoryName,
        double currentStock,
        double totalSold,
        double turnoverRate,
        string turnoverClassification)
        : this()
    {
        ProductId = productId;
        ProductName = productName;
        CategoryName = categoryName;
        CurrentStock = currentStock;
        TotalSold = totalSold;
        TurnoverRate = turnoverRate;
        TurnoverClassification = turnoverClassification;
    }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [MaxLength(50)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string CategoryName { get; set; } = string.Empty;

    public double CurrentStock { get; set; }

    public double TotalSold { get; set; }

    public double TurnoverRate { get; set; }

    [Required]
    [MaxLength(200)]
    public string TurnoverClassification { get; set; } = string.Empty;
}
