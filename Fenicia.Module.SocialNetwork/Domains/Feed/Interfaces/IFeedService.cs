using Fenicia.Common.DTOs.SocialNetwork.Feed;

namespace Fenicia.Module.SocialNetwork.Domains.Feed.Interfaces;

public interface IFeedService
{
    Task<List<FeedResponse>> GetAllAsync(GetAllFeedQuery query, CancellationToken cancellationToken = default);
    Task<List<FeedResponse>> GetByProfileIdAsync(GetFeedsByProfileQuery query, CancellationToken cancellationToken = default);
    Task<FeedResponse?> GetByIdAsync(GetFeedByIdQuery query, CancellationToken cancellationToken = default);
    Task<FeedResponse> AddAsync(FeedRequest command, Guid companyId, CancellationToken cancellationToken = default);
    Task<FeedResponse?> UpdateAsync(FeedRequest command, Guid companyId, CancellationToken cancellationToken = default);
    Task DeleteAsync(DeleteFeedCommand command, CancellationToken cancellationToken = default);
    Task IncrementTotalLikesAsync(Guid feedId, CancellationToken cancellationToken = default);
    Task DecrementTotalLikesAsync(Guid feedId, CancellationToken cancellationToken = default);
    Task IncrementTotalCommentsAsync(Guid feedId, CancellationToken cancellationToken = default);
    Task DecrementTotalCommentsAsync(Guid feedId, CancellationToken cancellationToken = default);
    Task IncrementTotalSharesAsync(Guid feedId, CancellationToken cancellationToken = default);
}