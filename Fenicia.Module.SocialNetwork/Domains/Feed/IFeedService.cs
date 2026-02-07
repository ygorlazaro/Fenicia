using Fenicia.Common.Data.Requests.SocialNetwork;
using Fenicia.Common.Data.Responses.SocialNetwork;

namespace Fenicia.Module.SocialNetwork.Domains.Feed;

public interface IFeedService
{
    Task<List<FeedResponse>> GetFollowingFeedAsync(Guid userId, CancellationToken ct, int page = 1, int perPage = 10);

    Task<FeedResponse> AddAsync(Guid userId, FeedRequest request, CancellationToken ct);
}