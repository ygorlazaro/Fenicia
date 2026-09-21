using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.DTOs.SocialNetwork.Feed;
using Fenicia.Module.SocialNetwork.Domains.Feed.Interfaces;

namespace Fenicia.Module.SocialNetwork.Domains.Feed;

public class FeedService(IFeedRepository repository) : IFeedService
{
    public async Task<List<FeedResponse>> GetAllAsync(
        GetAllFeedQuery query,
        CancellationToken cancellationToken = default)
    {
        var feeds = await repository.GetAllAsync(query.Page, query.PerPage, cancellationToken);
        return [.. feeds.Select(f => new FeedResponse(
            f.Id,
            f.Date,
            f.Text,
            f.ProfileId,
            f.CompanyId,
            f.TotalLikes,
            f.TotalComments,
            f.TotalShares,
            f.OriginalFeedId,
            f.Profile.UserName,
            f.Profile.Upload?.Url))];
    }

    public async Task<List<FeedResponse>> GetByProfileIdAsync(
        GetFeedsByProfileQuery query,
        CancellationToken cancellationToken = default)
    {
        var feeds = await repository.GetAllAsync(query.Page, query.PerPage, cancellationToken);
        var filtered = feeds.Where(f => f.ProfileId == query.ProfileId).ToList();
        return [.. filtered.Select(f => new FeedResponse(
            f.Id,
            f.Date,
            f.Text,
            f.ProfileId,
            f.CompanyId,
            f.TotalLikes,
            f.TotalComments,
            f.TotalShares,
            f.OriginalFeedId,
            f.Profile.UserName,
            f.Profile.Upload?.Url))];
    }

    public async Task<FeedResponse?> GetByIdAsync(
        GetFeedByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var feed = await repository.GetByIdWithRelationsAsync(query.Id, cancellationToken);
        return feed is null ? null : new FeedResponse(
            feed.Id,
            feed.Date,
            feed.Text,
            feed.ProfileId,
            feed.CompanyId,
            feed.TotalLikes,
            feed.TotalComments,
            feed.TotalShares,
            feed.OriginalFeedId,
            feed.Profile.UserName,
            feed.Profile.Upload?.Url);
    }

    public async Task<FeedResponse> AddAsync(
        FeedRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var model = new FeedModel
        {
            Id = command.Id ?? Guid.NewGuid(),
            Date = command.Date,
            Text = command.Text,
            ProfileId = command.ProfileId,
            OriginalFeedId = command.OriginalFeedId,
            CompanyId = companyId,
            TotalLikes = 0,
            TotalComments = 0,
            TotalShares = 0
        };

        var created = await repository.InsertAsync(model, cancellationToken);
        return new FeedResponse(
            created.Id,
            created.Date,
            created.Text,
            created.ProfileId,
            created.CompanyId,
            created.TotalLikes,
            created.TotalComments,
            created.TotalShares,
            created.OriginalFeedId,
            created.Profile.UserName,
            created.Profile.Upload?.Url);
    }

    public async Task<FeedResponse?> UpdateAsync(
        FeedRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByIdAsync(command.Id!.Value, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Date = command.Date;
        existing.Text = command.Text;
        existing.CompanyId = companyId;

        var updated = await repository.UpdateAsync(command.Id.Value, existing, cancellationToken);
        return updated is null ? null : new FeedResponse(
            updated.Id,
            updated.Date,
            updated.Text,
            updated.ProfileId,
            updated.CompanyId,
            updated.TotalLikes,
            updated.TotalComments,
            updated.TotalShares,
            updated.OriginalFeedId,
            updated.Profile.UserName,
            updated.Profile.Upload?.Url);
    }

    public async Task DeleteAsync(DeleteFeedCommand command, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(command.Id, cancellationToken);
    }

    public async Task IncrementTotalLikesAsync(Guid feedId, CancellationToken cancellationToken = default)
    {
        var feed = await repository.GetByIdAsync(feedId, cancellationToken);
        if (feed is null)
        {
            return;
        }

        feed.TotalLikes++;
        await repository.UpdateAsync(feedId, feed, cancellationToken);
    }

    public async Task DecrementTotalLikesAsync(Guid feedId, CancellationToken cancellationToken = default)
    {
        var feed = await repository.GetByIdAsync(feedId, cancellationToken);
        if (feed is null)
        {
            return;
        }

        feed.TotalLikes = Math.Max(0, feed.TotalLikes - 1);
        await repository.UpdateAsync(feedId, feed, cancellationToken);
    }

    public async Task IncrementTotalCommentsAsync(Guid feedId, CancellationToken cancellationToken = default)
    {
        var feed = await repository.GetByIdAsync(feedId, cancellationToken);
        if (feed is null)
        {
            return;
        }

        feed.TotalComments++;
        await repository.UpdateAsync(feedId, feed, cancellationToken);
    }

    public async Task DecrementTotalCommentsAsync(Guid feedId, CancellationToken cancellationToken = default)
    {
        var feed = await repository.GetByIdAsync(feedId, cancellationToken);
        if (feed is null)
        {
            return;
        }

        feed.TotalComments = Math.Max(0, feed.TotalComments - 1);
        await repository.UpdateAsync(feedId, feed, cancellationToken);
    }

    public async Task IncrementTotalSharesAsync(Guid feedId, CancellationToken cancellationToken = default)
    {
        var feed = await repository.GetByIdAsync(feedId, cancellationToken);
        if (feed is null)
        {
            return;
        }

        feed.TotalShares++;
        await repository.UpdateAsync(feedId, feed, cancellationToken);
    }
}
