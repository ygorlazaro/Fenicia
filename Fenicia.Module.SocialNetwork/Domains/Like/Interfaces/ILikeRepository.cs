using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.SocialNetwork.Domains.Like.Interfaces;

public interface ILikeRepository : IRepository<LikeModel>
{
    Task<LikeModel?> GetByProfileAndFeedAsync(Guid profileId, Guid feedId, CancellationToken cancellationToken = default);
    Task<List<LikeModel>> GetByProfileIdAsync(Guid profileId, int page, int perPage, CancellationToken cancellationToken = default);
}