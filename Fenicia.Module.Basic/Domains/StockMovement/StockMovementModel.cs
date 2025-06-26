namespace Fenicia.Module.Basic.Domains.StockMovement;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Common.Database;

using Product;

[Table(name: "stock_movements")]
public class StockMovementModel : BaseModel
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }

    public DateTime? Date { get; set; }

    public decimal Price { get; set; }

    [ForeignKey(name: "ProductId")]
    public virtual ProductModel Product { get; set; } = null!;
}
