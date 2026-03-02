using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models;

[Table("customers", Schema = "basic")]
public class BasicCustomer : BaseCompanyModel
{
    public List<BasicStockMovement> StockMovements { get; set; } = [];

    public List<BasicOrder> Orders { get; set; } = [];

    public Guid PersonId { get; set; }

    public BasicPerson Person { get; set; } = null!;
}
