using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models;

[Table("suppliers", Schema = "basic")]
public class BasicSupplier : BaseModel
{
    [MaxLength(14)]
    public string? Cnpj { get; set; }

    public Guid PersonId { get; set; } = Guid.Empty;

    public BasicPerson Person { get; set; } = null!;

    public List<BasicStockMovement> StockMovements { get; set; } = [];
}
