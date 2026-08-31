using Fenicia.Common;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Module.SocialNetwork.Domains.Feed.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Feed;

public class FeedService(FeedRepository repository)
{
    public async Task<List<GetAllFeedResponse>> GetAllAsync(GetAllFeedQuery query, CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query().OrderByDescending(f => f.Date);
        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);
        var feeds = await filteredQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(cancellationToken);
        return [.. feeds.Select(f => new GetAllFeedResponse(f.Id, f.Date, f.Text, f.UserId, f.CompanyId, f.Comments.Count, f.Likes.Count, f.Shares.Count))];
    }

    public async Task<GetFeedByIdResponse?> GetByIdAsync(GetFeedByIdQuery query, CancellationToken cancellationToken = default)
    {
        var feed = await repository.GetByIdWithRelationsAsync(query.Id, cancellationToken);

        return feed switch
        {
            null => null,
            _ => new GetFeedByIdResponse(feed.Id, feed.Date, feed.Text, feed.UserId, feed.CompanyId, feed.Comments.Count, feed.Likes.Count, feed.Shares.Count)
        };
    }

    public async Task<AddFeedResponse> AddAsync(AddFeedCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        var model = new FeedModel
        {
            Id = command.Id,
            Date = command.Date,
            Text = command.Text,
            UserId = command.UserId,
            CompanyId = companyId
        };

        var created = await repository.InsertAsync(model, cancellationToken);
        return new AddFeedResponse(created.Id, created.Date, created.Text, created.UserId, created.CompanyId);
    }

    public async Task<UpdateFeedResponse?> UpdateAsync(UpdateFeedCommand command, Guid companyId, CancellationToken cancellationToken = default)
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
        return updated is null ? null : new UpdateFeedResponse(updated.Id, updated.Date, updated.Text, updated.UserId, updated.CompanyId);
    }

    public async Task DeleteAsync(DeleteFeedCommand command, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(command.Id, cancellationToken);
    }
}
