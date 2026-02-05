using Fenicia.Common.Database.Converters.Basic;
using Fenicia.Common.Database.Requests.Basic;
using Fenicia.Common.Database.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Position;

public class PositionService(IPositionRepository positionRepository) : IPositionService
{
    public async Task<List<PositionResponse>> GetAllAsync(CancellationToken cancellationToken, int page = 1, int perPage = 1)
    {
        var positions = await positionRepository.GetAllAsync(cancellationToken, page, perPage);

        return PositionConverter.Convert(positions);
    }

    public async Task<PositionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var positin = await positionRepository.GetByIdAsync(id, cancellationToken);

        return positin is null ? null : PositionConverter.Convert(positin);
    }

    public async Task<PositionResponse?> AddAsync(PositionRequest request, CancellationToken cancellationToken)
    {
        var position = PositionConverter.Convert(request);

        positionRepository.Add(position);

        await positionRepository.SaveChangesAsync(cancellationToken);

        return PositionConverter.Convert(position);
    }

    public async Task<PositionResponse?> UpdateAsync(PositionRequest request, CancellationToken cancellationToken)
    {
        var position = PositionConverter.Convert(request);

        positionRepository.Update(position);

        await positionRepository.SaveChangesAsync(cancellationToken);

        return PositionConverter.Convert(position);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        positionRepository.Delete(id);

        await positionRepository.SaveChangesAsync(cancellationToken);
    }
}
