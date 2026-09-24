using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.Basic;

[Table("customers", Schema = "basic")]
public class CustomerModel : BaseCompanyModel
{
    public List<StockMovementModel> StockMovements { get; init; } = [];

    public List<OrderModel> Orders { get; init; } = [];

    public Guid PersonId { get; init; }

    public PersonModel Person { get; init; } = null!;
}
