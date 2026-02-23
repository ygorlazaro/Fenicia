using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Data.Requests.Basic;

namespace Fenicia.Common.Data.Models.Basic;

[Table("suppliers")]
public class SupplierModel(SupplierRequest request) : BaseModel
{
    [MaxLength(14)]
    public string? Cnpj { get; set; } = request.Cnpj;

    public Guid PersonId { get; set; } = Guid.Empty;

    public PersonModel Person { get; set; } = new(request.Person);

    public List<StockMovementModel> StockMovements { get; set; } = [];
}