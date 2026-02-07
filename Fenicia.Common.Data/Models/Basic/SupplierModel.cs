using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Basic;

[Table("suppliers")]
public class SupplierModel : BaseModel
{
    [MaxLength(14)]
    public string? Cnpj { get; set; } = null!;

    public Guid PersonId { get; set; }

    public virtual PersonModel Person
    {
        get;
        set;
    }

    public virtual List<StockMovementModel> StockMovements
    {
        get;
        set;
    }

    = [];
}