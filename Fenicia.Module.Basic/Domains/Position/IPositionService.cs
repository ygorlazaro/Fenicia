using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Position;

public interface IPositionService
{
    Task<List<PositionResponse>> GetAllAsync(CancellationToken ct, int page = 1, int perPage = 1);

    Task<PositionResponse?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<PositionResponse?> AddAsync(PositionRequest request, CancellationToken ct);

    Task<PositionResponse?> UpdateAsync(PositionRequest request, CancellationToken ct);

    Task DeleteAsync(Guid id, CancellationToken ct);
}
