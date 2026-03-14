using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Basic;

[Table("customers", Schema = "basic")]
public class CustomerModel : BaseCompanyModel
{
    public List<StockMovementModel> StockMovements { get; set; } = [];

    public List<OrderModel> Orders { get; set; } = [];

    public Guid PersonId { get; set; }

    public PersonModel Person { get; set; } = null!;
}