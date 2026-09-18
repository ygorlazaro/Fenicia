using Fenicia.Common;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.DTOs.SocialNetwork.Block;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Block;

public sealed class BlockService(IBlockRepository blockRepository, BlockMapper mapper)
{
    public BlockService()
        : this(null!, null!)
    {
    }

    public async Task<AddBlockResponse> BlockAsync(
        BlockCommand command,
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        var existing = await blockRepository.FindAsync(
            b => b.ProfileId == profileId && b.BlockedProfileId == command.BlockedProfileId,
            cancellationToken);

        var block = existing.FirstOrDefault();
        if (block is not null)
        {
            if (block.IsActive)
            {
                return mapper.MapToAddBlockResponse(block);
            }

            block.IsActive = true;
            block.BlockDate = DateTime.UtcNow;
            block.Reason = null;
            await blockRepository.UpdateAsync(block.Id, block, cancellationToken);
            return mapper.MapToAddBlockResponse(block);
        }

        var newBlock = new BlockModel
        {
            ProfileId = profileId,
            BlockedProfileId = command.BlockedProfileId,
            BlockDate = DateTime.UtcNow,
            IsActive = true
        };

        var created = await blockRepository.InsertAsync(newBlock, cancellationToken);
        return mapper.MapToAddBlockResponse(created);
    }

    public async Task UnblockAsync(
        UnblockCommand command,
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        var existing = await blockRepository.FindAsync(
            b => b.ProfileId == profileId && b.BlockedProfileId == command.BlockedProfileId && b.IsActive,
            cancellationToken);

        var block = existing.FirstOrDefault();
        if (block is not null)
        {
            block.IsActive = false;
            await blockRepository.UpdateAsync(block.Id, block, cancellationToken);
        }
    }

    public async Task<Pagination<List<GetBlockedResponse>>> GetBlockedAsync(
        GetBlockedQuery query,
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = blockRepository.Query().Where(b => b.ProfileId == profileId && b.IsActive);
        var total = await baseQuery.CountAsync(cancellationToken);
        var blocks = await baseQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage)
            .ToListAsync(cancellationToken);

        var response = blocks.Select(mapper.MapToGetBlockedResponse).ToList();

        return new Pagination<List<GetBlockedResponse>>(response, total, query.Page, query.PerPage);
    }

    public Task<bool> IsBlockedAsync(
        IsBlockedQuery query,
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        return blockRepository.AnyAsync(
            b => b.ProfileId == profileId && b.BlockedProfileId == query.BlockedProfileId && b.IsActive,
            cancellationToken);
    }
}
