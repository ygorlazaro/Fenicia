using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Common.Data.Converters.Basic;

public static class PositionConverter
{
    public static PositionModel Convert(PositionRequest request)
    {
        return new PositionModel
        {
            Id = request.Id,
            Name = request.Name
        };
    }

    public static PositionResponse Convert(PositionModel model)
    {
        return new PositionResponse
        {
            Id = model.Id,
            Name = model.Name
        };
    }

    public static List<PositionResponse> Convert(List<PositionModel> models)
    {
        return models.Select(Convert).ToList();
    }
}
