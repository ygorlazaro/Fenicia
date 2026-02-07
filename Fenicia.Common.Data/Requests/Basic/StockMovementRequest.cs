using System.ComponentModel.DataAnnotations;

using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.Data.Requests.Basic;

public class StockMovementRequest
{
    public Guid Id { get; set; }

    [Required]
    [Range(0.001, double.MaxValue)]
    public int Quantity { get; set; }

    public DateTime? Date { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public StockMovementType Type { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? SupplierId { get; set; }
}