using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Module.SocialNetwork.Domains.Feed.DTOs;

namespace Fenicia.Module.SocialNetwork.Domains.Feed;

public class FeedService(FeedRepository repository)
{
    public async Task<List<GetAllFeedResponse>> GetAllAsync(GetAllFeedQuery query, CancellationToken ct)
    {
        var feeds = await repository.GetAllAsync(query.Page, query.PerPage, ct);
        return [.. feeds.Select(f => new GetAllFeedResponse(f.Id, f.Date, f.Text, f.UserId, f.CompanyId, f.Comments.Count, f.Likes.Count, f.Shares.Count))];
    }

    public async Task<GetFeedByIdResponse?> GetByIdAsync(GetFeedByIdQuery query, CancellationToken ct)
    {
        var feed = await repository.GetByIdWithRelationsAsync(query.Id, ct);

        return feed switch
        {
            null => null,
            _ => new GetFeedByIdResponse(feed.Id, feed.Date, feed.Text, feed.UserId, feed.CompanyId, feed.Comments.Count, feed.Likes.Count, feed.Shares.Count)
        };
    }

    public async Task<AddFeedResponse> AddAsync(AddFeedCommand command, Guid companyId, CancellationToken ct)
    {
        var model = new FeedModel
        {
            Id = command.Id,
            Date = command.Date,
            Text = command.Text,
            UserId = command.UserId,
            CompanyId = companyId
        };

        var created = await repository.InsertAsync(model, ct);
        return new AddFeedResponse(created.Id, created.Date, created.Text, created.UserId, created.CompanyId);
    }

    public async Task<UpdateFeedResponse?> UpdateAsync(UpdateFeedCommand command, Guid companyId, CancellationToken ct)
    {
        var existing = await repository.GetByIdAsync(command.Id, ct);
        if (existing is null)
        {
            return null;
        }

        existing.Date = command.Date;
        existing.Text = command.Text;
        existing.CompanyId = companyId;

        var updated = await repository.UpdateAsync(command.Id, existing, ct);
        return updated is null ? null : new UpdateFeedResponse(updated.Id, updated.Date, updated.Text, updated.UserId, updated.CompanyId);
    }

    public async Task DeleteAsync(DeleteFeedCommand command, CancellationToken ct)
    {
        await repository.DeleteAsync(command.Id, ct);
    }
}
