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

    public virtual async Task<AddBlockResponse> BlockAsync(BlockCommand command, Guid userId, CancellationToken cancellationToken = default)
    {
        var existing = await blockRepository.FindAsync(
            b => b.UserId == userId && b.BlockedUserId == command.BlockedUserId, cancellationToken);

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
            await blockRepository.UpdateAsync(block.Id, block, cancellationToken);
            return new AddBlockResponse(block.Id, block.UserId, block.BlockedUserId, block.BlockDate, block.Reason, block.IsActive);
        }

        var newBlock = new BlockModel
        {
            UserId = userId,
            BlockedUserId = command.BlockedUserId,
            BlockDate = DateTime.UtcNow,
            IsActive = true
        };

        var created = await blockRepository.InsertAsync(newBlock, cancellationToken);
        return new AddBlockResponse(created.Id, created.UserId, created.BlockedUserId, created.BlockDate, created.Reason, created.IsActive);
    }

    public virtual async Task UnblockAsync(UnblockCommand command, Guid userId, CancellationToken cancellationToken = default)
    {
        var existing = await blockRepository.FindAsync(
            b => b.UserId == userId && b.BlockedUserId == command.BlockedUserId && b.IsActive, cancellationToken);

        var block = existing.FirstOrDefault();
        if (block is not null)
        {
            block.IsActive = false;
            await blockRepository.UpdateAsync(block.Id, block, cancellationToken);
        }
    }

    public virtual async Task<Pagination<List<GetBlockedResponse>>> GetBlockedAsync(GetBlockedQuery query, Guid userId, CancellationToken cancellationToken = default)
    {
        var baseQuery = blockRepository.Query().Where(b => b.UserId == userId && b.IsActive);
        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);
        var total = await filteredQuery.CountAsync(cancellationToken);
        var blocks = await filteredQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(cancellationToken);

        var response = blocks.Select(b => new GetBlockedResponse(b.Id, b.BlockedUserId, b.BlockDate, b.Reason)).ToList();

        return new Pagination<List<GetBlockedResponse>>(response, total, query.Page, query.PerPage);
    }

    public virtual async Task<bool> IsBlockedAsync(IsBlockedQuery query, Guid userId, CancellationToken cancellationToken = default)
    {
        return await blockRepository.AnyAsync(
            b => b.UserId == userId && b.BlockedUserId == query.BlockedUserId && b.IsActive, cancellationToken);
    }
}
