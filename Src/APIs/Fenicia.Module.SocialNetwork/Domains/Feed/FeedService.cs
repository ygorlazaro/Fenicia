using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Module.SocialNetwork.Domains.Feed.DTOs;

namespace Fenicia.Module.SocialNetwork.Domains.Feed;

public class FeedService(FeedRepository repository)
{
    public async Task<List<GetAllFeedResponse>> GetAllAsync(
        GetAllFeedQuery query,
        CancellationToken cancellationToken = default)
    {
        var feeds = await repository.GetAllAsync(query.Page, query.PerPage, cancellationToken);
        return
        [
            .. feeds.Select(f => new GetAllFeedResponse(
                f.Id,
                f.Date,
                f.Text,
                f.ProfileId,
                f.CompanyId,
                f.TotalLikes,
                f.TotalComments,
                f.TotalShares,
                f.OriginalFeedId,
                f.Profile?.UserName,
                f.Profile?.Upload?.Url))
        ];
    }

    public async Task<List<GetAllFeedResponse>> GetByProfileIdAsync(
        GetFeedsByProfileQuery query,
        CancellationToken cancellationToken = default)
    {
        var feeds = await repository.GetAllAsync(query.Page, query.PerPage, cancellationToken);
        var filtered = feeds.Where(f => f.ProfileId == query.ProfileId).ToList();
        return
        [
            .. filtered.Select(f => new GetAllFeedResponse(
                f.Id,
                f.Date,
                f.Text,
                f.ProfileId,
                f.CompanyId,
                f.TotalLikes,
                f.TotalComments,
                f.TotalShares,
                f.OriginalFeedId,
                f.Profile?.UserName,
                f.Profile?.Upload?.Url))
        ];
    }

    public async Task<GetFeedByIdResponse?> GetByIdAsync(
        GetFeedByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var feed = await repository.GetByIdWithRelationsAsync(query.Id, cancellationToken);

        return feed switch
        {
            null => null,
            _ => new GetFeedByIdResponse(
                feed.Id,
                feed.Date,
                feed.Text,
                feed.ProfileId,
                feed.CompanyId,
                feed.TotalLikes,
                feed.TotalComments,
                feed.TotalShares,
                feed.OriginalFeedId,
                feed.Profile?.UserName,
                feed.Profile?.Upload?.Url)
        };
    }

    public async Task<AddFeedResponse> AddAsync(
        AddFeedCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var model = new FeedModel
        {
            Id = command.Id,
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
        return new AddFeedResponse(created.Id, created.Date, created.Text, created.ProfileId, created.CompanyId, created.OriginalFeedId);
    }

    public async Task<UpdateFeedResponse?> UpdateAsync(
        UpdateFeedCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Date = command.Date;
        existing.Text = command.Text;
        existing.CompanyId = companyId;

        var updated = await repository.UpdateAsync(command.Id, existing, cancellationToken);
        return updated is null
            ? null
            : new UpdateFeedResponse(updated.Id, updated.Date, updated.Text, updated.ProfileId, updated.CompanyId, updated.OriginalFeedId);
    }

    public async Task DeleteAsync(DeleteFeedCommand command, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(command.Id, cancellationToken);
    }
}
