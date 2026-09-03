using Fenicia.Common;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Module.SocialNetwork.Domains.Like.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Like;

public class LikeService(LikeRepository repository)
{
    public async Task<AddLikeResponse> LikeAsync(
        LikeCommand command,
        Guid companyId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByUserAndFeedAsync(userId, command.FeedId, cancellationToken);
        if (existing is not null)
        {
            return new AddLikeResponse(
                existing.Id,
                existing.UserId,
                existing.FeedId,
                existing.LikeDate,
                existing.CompanyId);
        }

        var model = new LikeModel
        {
            UserId = userId,
            FeedId = command.FeedId,
            LikeDate = DateTime.UtcNow,
            CompanyId = companyId
        };

        var created = await repository.InsertAsync(model, cancellationToken);
        return new AddLikeResponse(created.Id, created.UserId, created.FeedId, created.LikeDate, created.CompanyId);
    }

    public async Task UnlikeAsync(UnlikeCommand command, Guid userId, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByUserAndFeedAsync(userId, command.FeedId, cancellationToken);
        if (existing is not null)
        {
            await repository.DeleteAsync(existing.Id, cancellationToken);
        }
    }

    public async Task<List<GetLikesResponse>> GetLikesByFeedAsync(
        GetLikesByFeedQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query().Where(l => l.FeedId == query.FeedId).OrderByDescending(l => l.LikeDate);
        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);
        var likes = await filteredQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage)
            .ToListAsync(cancellationToken);
        return [.. likes.Select(l => new GetLikesResponse(l.Id, l.UserId, l.FeedId, l.LikeDate))];
    }

    public async Task<bool> IsLikedAsync(
        IsLikedQuery query,
        Guid userId,
        Guid feedId,
        CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByUserAndFeedAsync(userId, feedId, cancellationToken);
        return existing is not null;
    }
}