using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Basic;

[Table("products")]
public class ProductModel : BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Name
    {
        get;
        set;
    }

    = null!;

    public decimal? CostPrice
    {
        get;
        set;
    }

    [Required]
    public decimal SalesPrice
    {
        get;
        set;
    }

    [Required]
    public double Quantity
    {
        get;
        set;
    }

    [Required]
    public Guid CategoryId
    {
        get;
        set;
    }

    [ForeignKey(nameof(CategoryId))]
    public virtual ProductCategoryModel Category
    {
        get;
        set;
    }

    = null!;

    public virtual List<StockMovementModel> StockMovements
    {
        get;
        set;
    }

    = null!;

    public virtual List<OrderDetailModel> OrderDetails
    {
        get;
        set;
    }

    = null;
}
