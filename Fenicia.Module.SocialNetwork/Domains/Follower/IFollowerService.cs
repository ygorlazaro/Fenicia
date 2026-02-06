using Fenicia.Common.Data.Responses.SocialNetwork;

namespace Fenicia.Module.SocialNetwork.Domains.Follower;

public interface IFollowerService
{
    Task<FollowerResponse?> FollowAsync(Guid userId, Guid followerId, CancellationToken cancellationToken);

    Task<FollowerResponse?> UnfollowAsync(Guid userId, Guid followerId, CancellationToken cancellationToken);

    Task<List<FollowerResponse>> GetFollowersAsync(Guid userId, CancellationToken cancellationToken, int page = 1, int perPage = 10);

    Task<int> CountAsync(Guid userId, CancellationToken cancellationToken);
}
