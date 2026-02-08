using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Common.Data.Responses.Basic;

public class PositionResponse(PositionModel model)
{
    public Guid Id { get; set; } = model.Id;

    public string Name { get; set; } = model.Name;
}
