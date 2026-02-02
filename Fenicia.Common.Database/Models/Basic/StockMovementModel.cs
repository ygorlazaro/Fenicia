using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Database.Models.Basic;

[Table("stock_movements")]
public class StockMovementModel : BaseModel
{
    [Required]
    public Guid ProductId
    {
        get; set;
    }

    [Required]
    public int Quantity
    {
        get; set;
    }

    public DateTime? Date
    {
        get; set;
    }

    public decimal Price
    {
        get; set;
    }

    [ForeignKey(nameof(StockMovementModel.ProductId))]
    public virtual ProductModel Product { get; set; } = null!;
}
