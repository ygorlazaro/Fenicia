using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Position.DTOs;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.Position;

[Mapper]
public static partial class PositionMapper
{
    public static GetAllPositionResponse MapToGetAllPositionResponse(this PositionModel position)
    {
        return new GetAllPositionResponse(position.Id, position.Name);
    }

    public static GetPositionByIdResponse MapToGetPositionByIdResponse(this PositionModel position)
    {
        return new GetPositionByIdResponse(position.Id, position.Name);
    }

    public static AddPositionResponse MapToAddPositionResponse(this PositionModel position)
    {
        return new AddPositionResponse(position.Id, position.Name);
    }

    public static UpdatePositionResponse MapToUpdatePositionResponse(this PositionModel position)
    {
        return new UpdatePositionResponse(position.Id, position.Name);
    }
}