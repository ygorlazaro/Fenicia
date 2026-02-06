using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Models.SocialNetwork;

namespace Fenicia.Module.SocialNetwork.Domains.Feed;

public interface IFeedRepository: IBaseRepository<FeedModel>
{
    Task<List<FeedModel>> GetFollowingFeedAsync(Guid userId, CancellationToken cancellationToken, int page = 1, int perPage = 10);
}
