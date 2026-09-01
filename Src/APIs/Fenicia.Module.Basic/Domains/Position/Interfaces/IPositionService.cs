using Fenicia.Common;
using Fenicia.Module.Basic.Domains.Position.DTOs;

namespace Fenicia.Module.Basic.Domains.Position.Interfaces;

public interface IPositionService
{
    Task<Pagination<List<GetAllPositionResponse>>> GetAllAsync(GetAllPositionQuery query, CancellationToken cancellationToken = default);

    Task<GetPositionByIdResponse?> GetByIdAsync(GetPositionByIdQuery query, CancellationToken cancellationToken = default);

    Task<AddPositionResponse> AddAsync(AddPositionCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task<UpdatePositionResponse?> UpdateAsync(UpdatePositionCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task DeleteAsync(DeletePositionCommand command, Guid companyId, CancellationToken cancellationToken = default);
}
