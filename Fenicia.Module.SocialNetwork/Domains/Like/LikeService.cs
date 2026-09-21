using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.DTOs.SocialNetwork.Feed;
using Fenicia.Common.DTOs.SocialNetwork.Like;
using Fenicia.Module.SocialNetwork.Domains.Feed.Interfaces;
using Fenicia.Module.SocialNetwork.Domains.Like.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Like;

public class LikeService(ILikeRepository repository, IFeedService feedService) : ILikeService
{
    public async Task<AddLikeResponse> LikeAsync(
        LikeCommand command,
        Guid companyId,
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByProfileAndFeedAsync(profileId, command.FeedId, cancellationToken);
        if (existing is not null)
        {
            return new AddLikeResponse(
                existing.Id,
                existing.ProfileId,
                existing.FeedId,
                existing.LikeDate,
                existing.CompanyId);
        }

        var model = new LikeModel
        {
            ProfileId = profileId,
            FeedId = command.FeedId,
            LikeDate = DateTime.UtcNow,
            CompanyId = companyId
        };

        var created = await repository.InsertAsync(model, cancellationToken);
        await feedService.IncrementTotalLikesAsync(command.FeedId, cancellationToken);
        return new AddLikeResponse(
            created.Id,
            created.ProfileId,
            created.FeedId,
            created.LikeDate,
            created.CompanyId);
    }

    public async Task UnlikeAsync(UnlikeCommand command, Guid profileId, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByProfileAndFeedAsync(profileId, command.FeedId, cancellationToken);
        if (existing is not null)
        {
            await repository.DeleteAsync(existing.Id, cancellationToken);
            await feedService.DecrementTotalLikesAsync(command.FeedId, cancellationToken);
        }
    }

    public async Task<List<GetLikesResponse>> GetLikesByFeedAsync(
        GetLikesByFeedQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query().Where(l => l.FeedId == query.FeedId).OrderByDescending(l => l.LikeDate);
        var likes = await baseQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage)
            .ToListAsync(cancellationToken);
        return [.. likes.Select(l => new GetLikesResponse(
            l.Id,
            l.ProfileId,
            l.FeedId,
            l.LikeDate))];
    }

    public async Task<bool> IsLikedAsync(
        IsLikedQuery query,
        Guid profileId,
        Guid feedId,
        CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByProfileAndFeedAsync(profileId, feedId, cancellationToken);
        return existing is not null;
    }

    public async Task<List<GetLikedFeedsResponse>> GetLikedFeedsByProfileAsync(
        GetLikedFeedsByProfileQuery query,
        CancellationToken cancellationToken = default)
    {
        var likes = await repository.GetByProfileIdAsync(
            query.ProfileId,
            query.Page,
            query.PerPage,
            cancellationToken);

        if (likes.Count == 0)
        {
            return [];
        }

        var feedIds = likes.Select(l => l.FeedId).ToList();
        var feeds = await feedService.GetAllAsync(new GetAllFeedQuery(1, feedIds.Count), cancellationToken);
        var feedDict = feeds.ToDictionary(f => f.Id);

        var likeDateByFeed = likes.ToDictionary(l => l.FeedId, l => l.LikeDate);

        return
        [
            .. likes
                .Where(l => feedDict.ContainsKey(l.FeedId))
                .OrderByDescending(l => likeDateByFeed[l.FeedId])
                .Select(l =>
                {
                    var feed = feedDict[l.FeedId];
                    return new GetLikedFeedsResponse(
                        feed.Id,
                        feed.Date,
                        feed.Text,
                        feed.ProfileId,
                        feed.CompanyId,
                        feed.TotalLikes,
                        feed.TotalComments,
                        feed.TotalShares);
                })
        ];
    }
}
