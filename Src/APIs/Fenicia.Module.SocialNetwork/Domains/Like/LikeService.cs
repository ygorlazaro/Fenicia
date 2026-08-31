using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Module.SocialNetwork.Domains.Like.DTOs;

namespace Fenicia.Module.SocialNetwork.Domains.Like;

public class LikeService(LikeRepository repository)
{
    public async Task<AddLikeResponse> LikeAsync(LikeCommand command, Guid companyId, Guid userId, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByUserAndFeedAsync(userId, command.FeedId, cancellationToken);
        if (existing is not null)
        {
            return new AddLikeResponse(existing.Id, existing.UserId, existing.FeedId, existing.LikeDate, existing.CompanyId);
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

    public async Task<List<GetLikesResponse>> GetLikesByFeedAsync(GetLikesByFeedQuery query, CancellationToken cancellationToken = default)
    {
        var likes = await repository.GetByFeedAsync(query.FeedId, query.Page, query.PerPage, cancellationToken);
        return [.. likes.Select(l => new GetLikesResponse(l.Id, l.UserId, l.FeedId, l.LikeDate))];
    }

    public async Task<bool> IsLikedAsync(IsLikedQuery query, Guid userId, Guid feedId, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByUserAndFeedAsync(userId, feedId, cancellationToken);
        return existing is not null;
    }
}
