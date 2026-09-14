using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Basic;

[Table("suppliers", Schema = "basic")]
public sealed class SupplierModel : BaseCompanyModel
{
    [MaxLength(14)]
    public string? Cnpj { get; set; }

    public Guid PersonId { get; init; } = Guid.Empty;

    public PersonModel Person { get; init; } = default!;

    public List<ProductModel> Products { get; init; } = [];

    public List<StockMovementModel> StockMovements { get; init; } = [];
}