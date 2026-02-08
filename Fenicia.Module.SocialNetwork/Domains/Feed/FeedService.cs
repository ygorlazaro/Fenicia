using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Requests.SocialNetwork;
using Fenicia.Common.Data.Responses.SocialNetwork;

namespace Fenicia.Module.SocialNetwork.Domains.Feed;

public class FeedService(IFeedRepository feedRepository) : IFeedService
{
    public async Task<List<FeedResponse>> GetFollowingFeedAsync(
        Guid userId,
        CancellationToken ct,
        int page = 1,
        int perPage = 10)
    {
        var feed = await feedRepository.GetFollowingFeedAsync(userId, ct, page, perPage);

        return [..feed.Select(f => new FeedResponse(f))];
    }

    public async Task<FeedResponse> AddAsync(Guid userId, FeedRequest request, CancellationToken ct)
    {
        request.UserId = userId;
        var model = new FeedModel(request);

        feedRepository.Add(model);

        await feedRepository.SaveChangesAsync(ct);

        return new FeedResponse(model);
    }
}