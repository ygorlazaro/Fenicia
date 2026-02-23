using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Data.Requests.Basic;

namespace Fenicia.Common.Data.Models.Basic;

[Table("customers")]
public class CustomerModel : BaseModel
{
    public CustomerModel(CustomerRequest request)
    {
        this.Person = new PersonModel(request.Person);
        this.Id = request.Id;
    }

    public List<StockMovementModel> StockMovements { get; set; } = [];

    public List<OrderModel> Orders { get; set; } = [];

    public Guid PersonId { get; set; }

    public PersonModel Person { get; set; }
}