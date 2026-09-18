using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Product;

public class ProfitMarginResponse
{
    public ProfitMarginResponse()
    {
    }

    public ProfitMarginResponse(
        Guid productId,
        string productName,
        string categoryName,
        decimal costPrice,
        decimal salesPrice,
        decimal profitMargin,
        string marginClassification)
    {
        ProductId = productId;
        ProductName = productName;
        CategoryName = categoryName;
        CostPrice = costPrice;
        SalesPrice = salesPrice;
        ProfitMargin = profitMargin;
        MarginClassification = marginClassification;
    }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string CategoryName { get; set; } = string.Empty;

    public decimal CostPrice { get; set; }

    public decimal SalesPrice { get; set; }

    public decimal ProfitMargin { get; set; }

    [Required]
    [MaxLength(200)]
    public string MarginClassification { get; set; } = string.Empty;
}
