using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.DTOs.SocialNetwork.Like;
using Fenicia.Module.SocialNetwork.Domains.Feed;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Like;

public class LikeService(LikeRepository likeRepository, FeedRepository feedRepository, LikeMapper mapper)
{
    public async Task<AddLikeResponse> LikeAsync(
        LikeCommand command,
        Guid companyId,
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        var existing = await likeRepository.GetByProfileAndFeedAsync(profileId, command.FeedId, cancellationToken);
        if (existing is not null)
        {
            return mapper.MapToAddLikeResponse(existing);
        }

        var model = new LikeModel
        {
            ProfileId = profileId,
            FeedId = command.FeedId,
            LikeDate = DateTime.UtcNow,
            CompanyId = companyId
        };

        var created = await likeRepository.InsertAsync(model, cancellationToken);
        await IncrementFeedTotalLikesAsync(command.FeedId, cancellationToken);
        return mapper.MapToAddLikeResponse(created);
    }

    public async Task UnlikeAsync(UnlikeCommand command, Guid profileId, CancellationToken cancellationToken = default)
    {
        var existing = await likeRepository.GetByProfileAndFeedAsync(profileId, command.FeedId, cancellationToken);
        if (existing is not null)
        {
            await likeRepository.DeleteAsync(existing.Id, cancellationToken);
            await DecrementFeedTotalLikesAsync(command.FeedId, cancellationToken);
        }
    }

    public async Task<List<GetLikesResponse>> GetLikesByFeedAsync(
        GetLikesByFeedQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = likeRepository.Query().Where(l => l.FeedId == query.FeedId).OrderByDescending(l => l.LikeDate);
        var likes = await baseQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage)
            .ToListAsync(cancellationToken);
        return [.. likes.Select(mapper.MapToGetLikesResponse)];
    }

    public async Task<bool> IsLikedAsync(
        IsLikedQuery query,
        Guid profileId,
        Guid feedId,
        CancellationToken cancellationToken = default)
    {
        var existing = await likeRepository.GetByProfileAndFeedAsync(profileId, feedId, cancellationToken);
        return existing is not null;
    }

    public async Task<List<GetLikedFeedsResponse>> GetLikedFeedsByProfileAsync(
        GetLikedFeedsByProfileQuery query,
        CancellationToken cancellationToken = default)
    {
        var likes = await likeRepository.GetByProfileIdAsync(
            query.ProfileId,
            query.Page,
            query.PerPage,
            cancellationToken);

        if (likes.Count == 0)
        {
            return [];
        }

        var feedIds = likes.Select(l => l.FeedId).ToList();
        var feeds = await feedRepository.Query()
            .Where(f => feedIds.Contains(f.Id))
            .ToDictionaryAsync(f => f.Id, cancellationToken);

        var likeDateByFeed = likes.ToDictionary(l => l.FeedId, l => l.LikeDate);

        return
        [
            .. likes
                .Where(l => feeds.ContainsKey(l.FeedId))
                .OrderByDescending(l => likeDateByFeed[l.FeedId])
                .Select(l => mapper.MapToGetLikedFeedsResponse(feeds[l.FeedId]))
        ];
    }

    private async Task IncrementFeedTotalLikesAsync(Guid feedId, CancellationToken cancellationToken)
    {
        var feed = await feedRepository.GetByIdAsync(feedId, cancellationToken);
        if (feed is null)
        {
            return;
        }

        feed.TotalLikes++;
        await feedRepository.UpdateAsync(feedId, feed, cancellationToken);
    }

    private async Task DecrementFeedTotalLikesAsync(Guid feedId, CancellationToken cancellationToken)
    {
        var feed = await feedRepository.GetByIdAsync(feedId, cancellationToken);
        if (feed is null)
        {
            return;
        }

        feed.TotalLikes = Math.Max(0, feed.TotalLikes - 1);
        await feedRepository.UpdateAsync(feedId, feed, cancellationToken);
    }
}
