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

    public virtual async Task<AddFriendshipResponse> FollowAsync(FollowCommand command, Guid userId, CancellationToken ct)
    {
        var existing = await friendshipRepository.FindAsync(
            f => f.UserId == userId && f.TargetUserId == command.TargetUserId, ct);

        var friendship = existing.FirstOrDefault();
        if (friendship is not null)
        {
            if (friendship.IsActive)
            {
                return new AddFriendshipResponse(friendship.Id, friendship.UserId, friendship.TargetUserId, friendship.FollowDate, friendship.IsActive);
            }

            friendship.IsActive = true;
            friendship.FollowDate = DateTime.UtcNow;
            await friendshipRepository.UpdateAsync(friendship.Id, friendship, ct);
            return new AddFriendshipResponse(friendship.Id, friendship.UserId, friendship.TargetUserId, friendship.FollowDate, friendship.IsActive);
        }

        var newFriendship = new FriendshipModel
        {
            UserId = userId,
            TargetUserId = command.TargetUserId,
            FollowDate = DateTime.UtcNow,
            IsActive = true
        };

        var created = await friendshipRepository.InsertAsync(newFriendship, ct);
        return new AddFriendshipResponse(created.Id, created.UserId, created.TargetUserId, created.FollowDate, created.IsActive);
    }

    public virtual async Task UnfollowAsync(UnfollowCommand command, Guid userId, CancellationToken ct)
    {
        var existing = await friendshipRepository.FindAsync(
            f => f.UserId == userId && f.TargetUserId == command.TargetUserId && f.IsActive, ct);

        var friendship = existing.FirstOrDefault();
        if (friendship is not null)
        {
            friendship.IsActive = false;
            await friendshipRepository.UpdateAsync(friendship.Id, friendship, ct);
        }
    }

    public virtual async Task<Pagination<List<GetFollowersResponse>>> GetFollowersAsync(GetFollowersQuery query, Guid targetUserId, CancellationToken ct)
    {
        var total = await friendshipRepository.CountAsync(f => f.TargetUserId == targetUserId && f.IsActive, ct);

        var friendships = await friendshipRepository.Query()
            .Where(f => f.TargetUserId == targetUserId && f.IsActive)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        var response = friendships.Select(f => new GetFollowersResponse(f.Id, f.UserId, f.FollowDate)).ToList();

        return new Pagination<List<GetFollowersResponse>>(response, total, query.Page, query.PerPage);
    }

    public virtual async Task<Pagination<List<GetFollowingResponse>>> GetFollowingAsync(GetFollowingQuery query, Guid userId, CancellationToken ct)
    {
        var total = await friendshipRepository.CountAsync(f => f.UserId == userId && f.IsActive, ct);

        var friendships = await friendshipRepository.Query()
            .Where(f => f.UserId == userId && f.IsActive)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        var response = friendships.Select(f => new GetFollowingResponse(f.Id, f.TargetUserId, f.FollowDate)).ToList();

        return new Pagination<List<GetFollowingResponse>>(response, total, query.Page, query.PerPage);
    }

    public virtual async Task<bool> IsFollowingAsync(IsFollowingQuery query, Guid userId, CancellationToken ct)
    {
        return await friendshipRepository.AnyAsync(
            f => f.UserId == userId && f.TargetUserId == query.TargetUserId && f.IsActive, ct);
    }
}
