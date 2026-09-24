using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Order;

public class OrderDetailRequest()
{
    public OrderDetailRequest(
        Guid productId,
        decimal price,
        double quantity,
        decimal discountAmount = 0)
        : this()
    {
        ProductId = productId;
        Price = price;
        Quantity = quantity;
        DiscountAmount = discountAmount;
    }

    [Required]
    public Guid ProductId { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0.01, double.MaxValue)]
    public double Quantity { get; set; }

    [Range(0, double.MaxValue)]
    public decimal DiscountAmount { get; set; }
}
