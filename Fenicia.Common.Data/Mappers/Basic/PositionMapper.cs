using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Common.Data.Mappers.Basic;

public static class PositionMapper
{
    public static PositionModel Map(PositionRequest request)
    {
        return new PositionModel
        {
            Id = request.Id,
            Name = request.Name
        };
    }

    public static PositionResponse Map(PositionModel model)
    {
        return new PositionResponse
        {
            Id = model.Id,
            Name = model.Name
        };
    }

    public static List<PositionResponse> Map(List<PositionModel> models)
    {
        return [.. models.Select(Map)];
    }
}