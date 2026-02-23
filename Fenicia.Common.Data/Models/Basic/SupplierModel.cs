using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Basic;

[Table("suppliers")]
public class SupplierModel : BaseModel
{
    [MaxLength(14)]
    public string? Cnpj { get; set; }

    public Guid PersonId { get; set; } = Guid.Empty;

    public PersonModel Person { get; set; } 

    public List<StockMovementModel> StockMovements { get; set; } = [];
}