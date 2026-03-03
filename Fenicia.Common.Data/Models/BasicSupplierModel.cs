using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models;

[Table("suppliers", Schema = "basic")]
public class BasicSupplierModel : BaseCompanyModel
{
    [MaxLength(14)]
    public string? Cnpj { get; set; }

    public Guid PersonId { get; set; } = Guid.Empty;

    public BasicPersonModel PersonModel { get; set; } = null!;

    public List<BasicStockMovementModel> StockMovements { get; set; } = [];
}
