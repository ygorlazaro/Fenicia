using Fenicia.Common;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Module.SocialNetwork.Domains.Friendship.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Friendship;

public sealed class FriendshipService(IFriendshipRepository friendshipRepository)
{
    public FriendshipService()
        : this(null!)
    {
    }

    public async Task<AddFriendshipResponse> FollowAsync(
        FollowCommand command,
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        var existing = await friendshipRepository.FindAsync(
            f => f.ProfileId == profileId && f.TargetProfileId == command.TargetProfileId,
            cancellationToken);

        var friendship = existing.FirstOrDefault();
        if (friendship is not null)
        {
            if (friendship.IsActive)
            {
                return new AddFriendshipResponse(
                    friendship.Id,
                    friendship.ProfileId,
                    friendship.TargetProfileId,
                    friendship.FollowDate,
                    friendship.IsActive);
            }

            friendship.IsActive = true;
            friendship.FollowDate = DateTime.UtcNow;
            await friendshipRepository.UpdateAsync(friendship.Id, friendship, cancellationToken);
            return new AddFriendshipResponse(
                friendship.Id,
                friendship.ProfileId,
                friendship.TargetProfileId,
                friendship.FollowDate,
                friendship.IsActive);
        }

        var newFriendship = new FriendshipModel
        {
            ProfileId = profileId,
            TargetProfileId = command.TargetProfileId,
            FollowDate = DateTime.UtcNow,
            IsActive = true
        };

        var created = await friendshipRepository.InsertAsync(newFriendship, cancellationToken);
        return new AddFriendshipResponse(
            created.Id,
            created.ProfileId,
            created.TargetProfileId,
            created.FollowDate,
            created.IsActive);
    }

    public async Task UnfollowAsync(
        UnfollowCommand command,
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        var existing = await friendshipRepository.FindAsync(
            f => f.ProfileId == profileId && f.TargetProfileId == command.TargetProfileId && f.IsActive,
            cancellationToken);

        var friendship = existing.FirstOrDefault();
        if (friendship is not null)
        {
            friendship.IsActive = false;
            await friendshipRepository.UpdateAsync(friendship.Id, friendship, cancellationToken);
        }
    }

    public async Task<Pagination<List<GetFollowersResponse>>> GetFollowersAsync(
        GetFollowersQuery query,
        Guid targetProfileId,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = friendshipRepository.Query().Where(f => f.TargetProfileId == targetProfileId && f.IsActive);
        var filteredQuery = baseQuery;
        var total = await filteredQuery.CountAsync(cancellationToken);

        var friendships = await filteredQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage)
            .ToListAsync(cancellationToken);

        var response = friendships.Select(f => new GetFollowersResponse(f.Id, f.ProfileId, f.FollowDate)).ToList();

        return new Pagination<List<GetFollowersResponse>>(response, total, query.Page, query.PerPage);
    }

    public async Task<Pagination<List<GetFollowingResponse>>> GetFollowingAsync(
        GetFollowingQuery query,
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = friendshipRepository.Query().Where(f => f.ProfileId == profileId && f.IsActive);
        var filteredQuery = baseQuery;
        var total = await filteredQuery.CountAsync(cancellationToken);

        var friendships = await filteredQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage)
            .ToListAsync(cancellationToken);

        var response = friendships.Select(f => new GetFollowingResponse(f.Id, f.TargetProfileId, f.FollowDate)).ToList();

        return new Pagination<List<GetFollowingResponse>>(response, total, query.Page, query.PerPage);
    }

    public Task<bool> IsFollowingAsync(
        IsFollowingQuery query,
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        return friendshipRepository.AnyAsync(
            f => f.ProfileId == profileId && f.TargetProfileId == query.TargetProfileId && f.IsActive,
            cancellationToken);
    }
}
