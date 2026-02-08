using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.Data.Requests.Basic;

public abstract class OrderDetailRequest
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public decimal Price { get; set; }

    public Guid OrderId { get; set; }

    public Guid Id { get; set; }

    [Required]
    public double Quantity { get; set; }
}