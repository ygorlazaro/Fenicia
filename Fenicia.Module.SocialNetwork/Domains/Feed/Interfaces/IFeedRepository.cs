using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.SocialNetwork.Domains.Feed.Interfaces;

public interface IFeedRepository : IRepository<FeedModel>
{
    Task<FeedModel?> GetByIdWithRelationsAsync(Guid id, CancellationToken cancellationToken = default);
}