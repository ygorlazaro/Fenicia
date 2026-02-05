using Fenicia.Common.Database.Requests.Basic;
using Fenicia.Common.Database.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Position;

public interface IPositionService
{
    Task<List<PositionResponse>> GetAllAsync(CancellationToken cancellationToken, int page = 1, int perPage = 1);

    Task<PositionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<PositionResponse?> AddAsync(PositionRequest request, CancellationToken cancellationToken);

    Task<PositionResponse?> UpdateAsync(PositionRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
