using Fenicia.Common;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Module.SocialNetwork.Domains.Friendship.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Friendship;

public class FriendshipService(IFriendshipRepository friendshipRepository)
{
    public FriendshipService()
        : this(null!)
    {
    }

    public virtual async Task<AddFriendshipResponse> FollowAsync(FollowCommand command, Guid userId, CancellationToken cancellationToken = default)
    {
        var existing = await friendshipRepository.FindAsync(
            f => f.UserId == userId && f.TargetUserId == command.TargetUserId, cancellationToken);

        var friendship = existing.FirstOrDefault();
        if (friendship is not null)
        {
            if (friendship.IsActive)
            {
                return new AddFriendshipResponse(friendship.Id, friendship.UserId, friendship.TargetUserId, friendship.FollowDate, friendship.IsActive);
            }

            friendship.IsActive = true;
            friendship.FollowDate = DateTime.UtcNow;
            await friendshipRepository.UpdateAsync(friendship.Id, friendship, cancellationToken);
            return new AddFriendshipResponse(friendship.Id, friendship.UserId, friendship.TargetUserId, friendship.FollowDate, friendship.IsActive);
        }

        var newFriendship = new FriendshipModel
        {
            UserId = userId,
            TargetUserId = command.TargetUserId,
            FollowDate = DateTime.UtcNow,
            IsActive = true
        };

        var created = await friendshipRepository.InsertAsync(newFriendship, cancellationToken);
        return new AddFriendshipResponse(created.Id, created.UserId, created.TargetUserId, created.FollowDate, created.IsActive);
    }

    public virtual async Task UnfollowAsync(UnfollowCommand command, Guid userId, CancellationToken cancellationToken = default)
    {
        var existing = await friendshipRepository.FindAsync(
            f => f.UserId == userId && f.TargetUserId == command.TargetUserId && f.IsActive, cancellationToken);

        var friendship = existing.FirstOrDefault();
        if (friendship is not null)
        {
            friendship.IsActive = false;
            await friendshipRepository.UpdateAsync(friendship.Id, friendship, cancellationToken);
        }
    }

    public virtual async Task<Pagination<List<GetFollowersResponse>>> GetFollowersAsync(GetFollowersQuery query, Guid targetUserId, CancellationToken cancellationToken = default)
    {
        var baseQuery = friendshipRepository.Query().Where(f => f.TargetUserId == targetUserId && f.IsActive);
        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);
        var total = await filteredQuery.CountAsync(cancellationToken);

        var friendships = await filteredQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(cancellationToken);

        var response = friendships.Select(f => new GetFollowersResponse(f.Id, f.UserId, f.FollowDate)).ToList();

        return new Pagination<List<GetFollowersResponse>>(response, total, query.Page, query.PerPage);
    }

    public virtual async Task<Pagination<List<GetFollowingResponse>>> GetFollowingAsync(GetFollowingQuery query, Guid userId, CancellationToken cancellationToken = default)
    {
        var baseQuery = friendshipRepository.Query().Where(f => f.UserId == userId && f.IsActive);
        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);
        var total = await filteredQuery.CountAsync(cancellationToken);

        var friendships = await filteredQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(cancellationToken);

        var response = friendships.Select(f => new GetFollowingResponse(f.Id, f.TargetUserId, f.FollowDate)).ToList();

        return new Pagination<List<GetFollowingResponse>>(response, total, query.Page, query.PerPage);
    }

    public virtual async Task<bool> IsFollowingAsync(IsFollowingQuery query, Guid userId, CancellationToken cancellationToken = default)
    {
        return await friendshipRepository.AnyAsync(
            f => f.UserId == userId && f.TargetUserId == query.TargetUserId && f.IsActive, cancellationToken);
    }
}
