using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.Data.Models.Basic;

[Table("stock_movements")]
public partial class StockMovementModel : BaseModel
{
    [Required]
    public Guid ProductId
    {
        get; set;
    }

    [Required]
    public double Quantity
    {
        get; set;
    }

    public DateTime? Date
    {
        get; set;
    }

    [Required]
    public decimal? Price
    {
        get; set;
    }

    [Required]
    public StockMovementType Type
    {
        get; set;
    }

    [ForeignKey(nameof(ProductId))]
    public virtual ProductModel Product { get; set; } = null!;

    public Guid? CustomerId
    {
        get;
        set;
    }

    public Guid? SupplierId
    {
        get;
        set;
    }

    [ForeignKey(nameof(CustomerId))]
    public virtual CustomerModel? Customer
    {
        get; set;
    }

    [ForeignKey(nameof(SupplierId))]
    public virtual SupplierModel? Supplier
    {
        get; set;
    }
}
