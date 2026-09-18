using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Order;

public class OrderDetailResponse()
{
    public OrderDetailResponse(
        Guid id,
        Guid productId,
        string productName,
        decimal price,
        decimal discountAmount,
        double quantity,
        decimal subtotal)
        : this()
    {
        Id = id;
        ProductId = productId;
        ProductName = productName;
        Price = price;
        DiscountAmount = discountAmount;
        Quantity = quantity;
        Subtotal = subtotal;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, double.MaxValue)]
    public decimal DiscountAmount { get; set; }

    [Range(0.01, double.MaxValue)]
    public double Quantity { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Subtotal { get; set; }
}
