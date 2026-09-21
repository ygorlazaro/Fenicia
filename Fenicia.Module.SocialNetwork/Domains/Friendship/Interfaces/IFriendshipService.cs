using Fenicia.Common;
using Fenicia.Common.DTOs.SocialNetwork.Friendship;

namespace Fenicia.Module.SocialNetwork.Domains.Friendship.Interfaces;

public interface IFriendshipService
{
    Task<AddFriendshipResponse> FollowAsync(FollowCommand command, Guid profileId, CancellationToken cancellationToken = default);
    Task UnfollowAsync(UnfollowCommand command, Guid profileId, CancellationToken cancellationToken = default);
    Task<Pagination<List<GetFollowersResponse>>> GetFollowersAsync(GetFollowersQuery query, Guid targetProfileId, CancellationToken cancellationToken = default);
    Task<Pagination<List<GetFollowingResponse>>> GetFollowingAsync(GetFollowingQuery query, Guid profileId, CancellationToken cancellationToken = default);
    Task<bool> IsFollowingAsync(IsFollowingQuery query, Guid profileId, CancellationToken cancellationToken = default);
}