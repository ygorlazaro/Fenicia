using Fenicia.Common;
using Fenicia.Common.DTOs.SocialNetwork.Block;

namespace Fenicia.Module.SocialNetwork.Domains.Block.Interfaces;

public interface IBlockService
{
    Task<BlockResponse> BlockAsync(BlockRequest command, Guid profileId, CancellationToken cancellationToken = default);
    Task UnblockAsync(BlockRequest command, Guid profileId, CancellationToken cancellationToken = default);
    Task<Pagination<List<BlockResponse>>> GetBlockedAsync(Guid profileId, int page = 1, int perPage = 10, CancellationToken cancellationToken = default);
    Task<bool> IsBlockedAsync(IsBlockedQuery query, Guid profileId, CancellationToken cancellationToken = default);
}