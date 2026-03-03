using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models;

[Table("customers", Schema = "basic")]
public class BasicCustomerModel : BaseCompanyModel
{
    public List<BasicStockMovementModel> StockMovements { get; set; } = [];

    public List<BasicOrderModel> Orders { get; set; } = [];

    public Guid PersonId { get; set; }

    public BasicPersonModel PersonModel { get; set; } = null!;
}
