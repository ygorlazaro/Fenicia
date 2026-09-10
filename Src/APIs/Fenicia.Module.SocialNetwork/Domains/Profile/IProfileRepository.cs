using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.SocialNetwork.Domains.Profile;

public interface IProfileRepository : IRepository<ProfileModel>
{
    Task<ProfileModel?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}