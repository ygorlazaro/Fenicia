using Fenicia.Common;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Module.SocialNetwork.Domains.Block.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Block;

public class BlockService(IBlockRepository blockRepository)
{
    public BlockService()
        : this(null!)
    {
    }

    public virtual async Task<AddBlockResponse> BlockAsync(BlockCommand command, Guid userId, CancellationToken ct)
    {
        var existing = await blockRepository.FindAsync(
            b => b.UserId == userId && b.BlockedUserId == command.BlockedUserId, ct);

        var block = existing.FirstOrDefault();
        if (block is not null)
        {
            if (block.IsActive)
            {
                return new AddBlockResponse(block.Id, block.UserId, block.BlockedUserId, block.BlockDate, block.Reason, block.IsActive);
            }

            block.IsActive = true;
            block.BlockDate = DateTime.UtcNow;
            block.Reason = null;
            await blockRepository.UpdateAsync(block.Id, block, ct);
            return new AddBlockResponse(block.Id, block.UserId, block.BlockedUserId, block.BlockDate, block.Reason, block.IsActive);
        }

        var newBlock = new BlockModel
        {
            UserId = userId,
            BlockedUserId = command.BlockedUserId,
            BlockDate = DateTime.UtcNow,
            IsActive = true
        };

        var created = await blockRepository.InsertAsync(newBlock, ct);
        return new AddBlockResponse(created.Id, created.UserId, created.BlockedUserId, created.BlockDate, created.Reason, created.IsActive);
    }

    public virtual async Task UnblockAsync(UnblockCommand command, Guid userId, CancellationToken ct)
    {
        var existing = await blockRepository.FindAsync(
            b => b.UserId == userId && b.BlockedUserId == command.BlockedUserId && b.IsActive, ct);

        var block = existing.FirstOrDefault();
        if (block is not null)
        {
            block.IsActive = false;
            await blockRepository.UpdateAsync(block.Id, block, ct);
        }
    }

    public virtual async Task<Pagination<List<GetBlockedResponse>>> GetBlockedAsync(GetBlockedQuery query, Guid userId, CancellationToken ct)
    {
        var total = await blockRepository.CountAsync(b => b.UserId == userId && b.IsActive, ct);

        var blocks = await blockRepository.Query()
            .Where(b => b.UserId == userId && b.IsActive)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        var response = blocks.Select(b => new GetBlockedResponse(b.Id, b.BlockedUserId, b.BlockDate, b.Reason)).ToList();

        return new Pagination<List<GetBlockedResponse>>(response, total, query.Page, query.PerPage);
    }

    public virtual async Task<bool> IsBlockedAsync(IsBlockedQuery query, Guid userId, CancellationToken ct)
    {
        return await blockRepository.AnyAsync(
            b => b.UserId == userId && b.BlockedUserId == query.BlockedUserId && b.IsActive, ct);
    }
}
