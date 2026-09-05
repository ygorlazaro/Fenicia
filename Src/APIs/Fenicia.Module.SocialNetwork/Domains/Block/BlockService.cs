using Fenicia.Common;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Module.SocialNetwork.Domains.Block.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Block;

public sealed class BlockService(IBlockRepository blockRepository)
{
    public BlockService()
        : this(null!)
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
                return new AddBlockResponse(
                    block.Id,
                    block.ProfileId,
                    block.BlockedProfileId,
                    block.BlockDate,
                    block.Reason,
                    block.IsActive);
            }

            block.IsActive = true;
            block.BlockDate = DateTime.UtcNow;
            block.Reason = null;
            await blockRepository.UpdateAsync(block.Id, block, cancellationToken);
            return new AddBlockResponse(
                block.Id,
                block.ProfileId,
                block.BlockedProfileId,
                block.BlockDate,
                block.Reason,
                block.IsActive);
        }

        var newBlock = new BlockModel
        {
            ProfileId = profileId,
            BlockedProfileId = command.BlockedProfileId,
            BlockDate = DateTime.UtcNow,
            IsActive = true
        };

        var created = await blockRepository.InsertAsync(newBlock, cancellationToken);
        return new AddBlockResponse(
            created.Id,
            created.ProfileId,
            created.BlockedProfileId,
            created.BlockDate,
            created.Reason,
            created.IsActive);
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
        var filteredQuery = baseQuery;
        var total = await filteredQuery.CountAsync(cancellationToken);
        var blocks = await filteredQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage)
            .ToListAsync(cancellationToken);

        var response = blocks.Select(b => new GetBlockedResponse(b.Id, b.BlockedProfileId, b.BlockDate, b.Reason))
            .ToList();

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
