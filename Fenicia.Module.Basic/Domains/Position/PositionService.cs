using Fenicia.Common.Data.Mappers.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Position;

public class PositionService(IPositionRepository positionRepository) : IPositionService
{
    public async Task<List<PositionResponse>> GetAllAsync(CancellationToken ct, int page = 1, int perPage = 1)
    {
        var positions = await positionRepository.GetAllAsync(ct, page, perPage);

        return PositionMapper.Map(positions);
    }

    public async Task<PositionResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var positin = await positionRepository.GetByIdAsync(id, ct);

        return positin is null ? null : PositionMapper.Map(positin);
    }

    public async Task<PositionResponse?> AddAsync(PositionRequest request, CancellationToken ct)
    {
        var position = PositionMapper.Map(request);

        positionRepository.Add(position);

        await positionRepository.SaveChangesAsync(ct);

        return PositionMapper.Map(position);
    }

    public async Task<PositionResponse?> UpdateAsync(PositionRequest request, CancellationToken ct)
    {
        var position = PositionMapper.Map(request);

        positionRepository.Update(position);

        await positionRepository.SaveChangesAsync(ct);

        return PositionMapper.Map(position);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        positionRepository.Delete(id);

        await positionRepository.SaveChangesAsync(ct);
    }
}