using Fenicia.Common;
using Fenicia.Common.DTOs.Basic.Position;

namespace Fenicia.Module.Basic.Domains.Position.Interfaces;

public interface IPositionService
{
    Task<Pagination<List<GetAllPositionResponse>>> GetAllAsync(
        GetAllPositionQuery query,
        CancellationToken cancellationToken = default);

    Task<GetPositionByIdResponse?> GetByIdAsync(
        GetPositionByIdQuery query,
        CancellationToken cancellationToken = default);

    Task<AddPositionResponse> AddAsync(
        AddPositionRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<UpdatePositionResponse?> UpdateAsync(
        UpdatePositionRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(DeletePositionRequest command, Guid companyId, CancellationToken cancellationToken = default);
}
