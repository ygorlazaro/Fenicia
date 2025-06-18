using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Database;
using Fenicia.Module.Basic.Domains.Product;

namespace Fenicia.Module.Basic.Domains.StockMovement;

[Table("stock_movements")]
public class StockMovementModel : BaseModel
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }

    public DateTime? Date { get; set; }

    public decimal Price { get; set; }

    [ForeignKey("ProductId")]
    public virtual ProductModel Product { get; set; } = null!;
}
