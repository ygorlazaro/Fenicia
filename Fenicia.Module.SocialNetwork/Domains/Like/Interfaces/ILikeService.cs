using Fenicia.Common.DTOs.SocialNetwork.Like;

namespace Fenicia.Module.SocialNetwork.Domains.Like.Interfaces;

public interface ILikeService
{
    Task<AddLikeResponse> LikeAsync(LikeCommand command, Guid companyId, Guid profileId, CancellationToken cancellationToken = default);
    Task UnlikeAsync(UnlikeCommand command, Guid profileId, CancellationToken cancellationToken = default);
    Task<List<GetLikesResponse>> GetLikesByFeedAsync(GetLikesByFeedQuery query, CancellationToken cancellationToken = default);
    Task<bool> IsLikedAsync(IsLikedQuery query, Guid profileId, Guid feedId, CancellationToken cancellationToken = default);
    Task<List<GetLikedFeedsResponse>> GetLikedFeedsByProfileAsync(GetLikedFeedsByProfileQuery query, CancellationToken cancellationToken = default);
}