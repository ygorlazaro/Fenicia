using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Order;

/// <summary>
/// Response DTO containing order detail data.
/// </summary>
/// <param name="id">The unique identifier of the order detail.</param>
/// <param name="orderId">The unique identifier of the order.</param>
/// <param name="productId">The unique identifier of the product.</param>
/// <param name="productName">The name of the product.</param>
/// <param name="price">The price of the product.</param>
/// <param name="discountAmount">The discount amount.</param>
/// <param name="quantity">The quantity ordered.</param>
/// <param name="subtotal">The subtotal (price * quantity - discount).</param>
public record OrderDetailResponse(
    [Required] Guid Id,
    [Required] Guid OrderId,
    [Required] Guid ProductId,
    [Required] [MaxLength(200)] string ProductName,
    [Range(0.01, double.MaxValue)] decimal Price,
    [Range(0, double.MaxValue)] decimal DiscountAmount,
    [Range(0.01, double.MaxValue)] double Quantity,
    [Range(0, double.MaxValue)] decimal Subtotal);