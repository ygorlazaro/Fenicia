using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

/// <summary>
/// Fenicia.Module.SocialNetwork/Domains/Profile/Interfaces/IProfileRepository.cs implementation.
/// </summary>

namespace Fenicia.Module.SocialNetwork.Domains.Profile.Interfaces;

public interface IProfileRepository : IRepository<ProfileModel>
{
    Task<ProfileModel?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}