using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Common.Data.Responses.Basic;

public class CustomerResponse(CustomerModel model)
{
    public Guid Id { get; set; } = model.Id;

    public PersonResponse Person { get; set; } = new(model.Person);
}