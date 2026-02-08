using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Position;

public class PositionService(IPositionRepository positionRepository) : IPositionService
{
    public async Task<List<PositionResponse>> GetAllAsync(CancellationToken ct, int page = 1, int perPage = 1)
    {
        var positions = await positionRepository.GetAllAsync(ct, page, perPage);

        return [.. positions.Select(p => new PositionResponse(p))];
    }

    public async Task<PositionResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var position = await positionRepository.GetByIdAsync(id, ct);

        return position is null ? null : new PositionResponse(position);
    }

    public async Task<PositionResponse?> AddAsync(PositionRequest request, CancellationToken ct)
    {
        var position = new PositionModel(request);

        positionRepository.Add(position);

        await positionRepository.SaveChangesAsync(ct);

        return new PositionResponse(position);
    }

    public async Task<PositionResponse?> UpdateAsync(PositionRequest request, CancellationToken ct)
    {
        var position = new PositionModel(request);

        positionRepository.Update(position);

        await positionRepository.SaveChangesAsync(ct);

        return new PositionResponse(position);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        await positionRepository.DeleteAsync(id, ct);

        await positionRepository.SaveChangesAsync(ct);
    }
}