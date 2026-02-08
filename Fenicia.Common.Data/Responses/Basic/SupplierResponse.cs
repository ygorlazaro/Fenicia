using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Common.Data.Responses.Basic;

public class SupplierResponse(SupplierModel model)
{
    public PersonResponse Person { get; set; } = new(model.Person);

    public Guid Id { get; set; } = model.Id;
}