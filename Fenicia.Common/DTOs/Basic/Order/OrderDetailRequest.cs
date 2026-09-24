using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Order;

/// <summary>
/// Request DTO for order detail.
/// </summary>
/// <param name="productId">The unique identifier of the product.</param>
/// <param name="price">The price of the product.</param>
/// <param name="quantity">The quantity ordered.</param>
/// <param name="discountAmount">The discount amount (optional).</param>
public record OrderDetailRequest(
    [Required] Guid ProductId,
    [Range(0.01, double.MaxValue)] decimal Price,
    [Range(0.01, double.MaxValue)] double Quantity,
    [Range(0, double.MaxValue)] decimal DiscountAmount = 0);