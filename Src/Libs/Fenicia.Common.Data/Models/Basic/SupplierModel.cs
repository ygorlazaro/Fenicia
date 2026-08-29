using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Basic;

[Table("suppliers", Schema = "basic")]
public class SupplierModel : BaseCompanyModel
{
    [MaxLength(14)]
    public string? Cnpj { get; set; }

    public Guid PersonId { get; set; } = Guid.Empty;

    public PersonModel Person { get; set; } = null!;

    public virtual List<ProductModel> Products { get; set; } = null!;

    public List<StockMovementModel> StockMovements { get; set; } = [];
}