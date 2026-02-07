using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Basic;

[Table("customers")]
public class CustomerModel : BaseModel
{
    public virtual List<StockMovementModel> StockMovements
    {
        get;
        set;
    }

        = [];

    public virtual List<OrderModel> Orders
    {
        get;
        set;
    } = [];

    public Guid PersonId
    {
        get;
        set;
    }

    public virtual PersonModel Person
    {
        get;
        set;
    }
}