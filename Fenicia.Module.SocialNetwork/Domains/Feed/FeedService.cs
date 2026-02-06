using Fenicia.Common.Data.Converters.SocialNetwork;
using Fenicia.Common.Data.Requests.SocialNetwork;
using Fenicia.Common.Data.Responses.SocialNetwork;

namespace Fenicia.Module.SocialNetwork.Domains.Feed;

public class FeedService(IFeedRepository feedRepository): IFeedService
{
    public async Task<List<FeedResponse>> GetFollowingFeedAsync(Guid userId, CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        var feed = await feedRepository.GetFollowingFeedAsync(userId, cancellationToken, page, perPage);

        return FeedConverter.Convert(feed);
    }

    public async Task<FeedResponse> AddAsync(Guid userId, FeedRequest request, CancellationToken cancellationToken)
    {
        request.UserId = userId;
        var model = FeedConverter.Convert(request);

        feedRepository.Add(model);

        await feedRepository.SaveChangesAsync(cancellationToken);

        return FeedConverter.Convert(model);
    }
}
