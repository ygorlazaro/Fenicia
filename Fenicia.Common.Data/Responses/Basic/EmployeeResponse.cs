using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Common.Data.Responses.Basic;

public class EmployeeResponse(EmployeeModel model)
{
    public Guid Id { get; set; } = model.Id;

    public Guid PositionId { get; set; } = model.PositionId;

    public PersonResponse Person { get; set; } = new(model.Person);
}
