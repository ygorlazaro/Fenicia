using Fenicia.Common;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.DTOs.SocialNetwork.Block;
using Fenicia.Module.SocialNetwork.Domains.Block.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Block;

public sealed class BlockService(IBlockRepository repository) : IBlockService
{
    public async Task<BlockResponse> BlockAsync(
        BlockRequest command,
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        var existing = await repository.FindAsync(
            b => b.ProfileId == profileId && b.BlockedProfileId == command.BlockedProfileId,
            cancellationToken);

        var block = existing.FirstOrDefault();
        if (block is not null)
        {
            if (block.IsActive)
            {
                return new BlockResponse(
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
            await repository.UpdateAsync(block.Id, block, cancellationToken);
            return new BlockResponse(
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

        var created = await repository.InsertAsync(newBlock, cancellationToken);
        return new BlockResponse(
            created.Id,
            created.ProfileId,
            created.BlockedProfileId,
            created.BlockDate,
            created.Reason,
            created.IsActive);
    }

    public async Task UnblockAsync(
        BlockRequest command,
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        var existing = await repository.FindAsync(
            b => b.ProfileId == profileId && b.BlockedProfileId == command.BlockedProfileId && b.IsActive,
            cancellationToken);

        var block = existing.FirstOrDefault();
        if (block is not null)
        {
            block.IsActive = false;
            await repository.UpdateAsync(block.Id, block, cancellationToken);
        }
    }

    public async Task<Pagination<List<BlockResponse>>> GetBlockedAsync(
        Guid profileId,
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query().Where(b => b.ProfileId == profileId && b.IsActive);
        var total = await baseQuery.CountAsync(cancellationToken);
        var blocks = await baseQuery.Skip((page - 1) * perPage).Take(perPage)
            .ToListAsync(cancellationToken);

        var response = blocks.Select(b => new BlockResponse(
            b.Id,
            b.ProfileId,
            b.BlockedProfileId,
            b.BlockDate,
            b.Reason,
            b.IsActive)).ToList();

        return new Pagination<List<BlockResponse>>(response, total, page, perPage);
    }

    public Task<bool> IsBlockedAsync(
        IsBlockedQuery query,
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        return repository.AnyAsync(
            b => b.ProfileId == profileId && b.BlockedProfileId == query.BlockedProfileId && b.IsActive,
            cancellationToken);
    }
}
