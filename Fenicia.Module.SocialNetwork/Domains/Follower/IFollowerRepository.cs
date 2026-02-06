using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Models.SocialNetwork;

namespace Fenicia.Module.SocialNetwork.Domains.Follower;

public interface IFollowerRepository: IBaseRepository<FollowerModel>
{
    Task<FollowerModel?> FindFollowerAsync(Guid userId, Guid followerId, CancellationToken cancellationToken);

    Task<List<FollowerModel>> GetFollowersAsync(Guid userId, CancellationToken cancellationToken, int page, int perPage);
}
