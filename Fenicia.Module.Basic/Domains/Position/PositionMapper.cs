using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.Position;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.Position;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class PositionMapper
{
    public partial GetAllPositionResponse MapToGetAllPositionResponse(PositionModel position);
    public partial GetPositionByIdResponse MapToGetPositionByIdResponse(PositionModel position);
    public partial AddPositionResponse MapToAddPositionResponse(PositionModel position);
    public partial UpdatePositionResponse MapToUpdatePositionResponse(PositionModel position);
}
