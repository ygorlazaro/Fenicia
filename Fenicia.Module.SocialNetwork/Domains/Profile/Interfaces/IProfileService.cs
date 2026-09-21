using Fenicia.Common.DTOs.SocialNetwork.Profile;

namespace Fenicia.Module.SocialNetwork.Domains.Profile.Interfaces;

public interface IProfileService
{
    Task<ProfileResponse?> GetByIdAsync(GetProfileByIdQuery query, CancellationToken cancellationToken = default);

    Task<ProfileResponse?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<ProfileResponse> CreateAsync(ProfileRequest command, Guid userId, CancellationToken cancellationToken = default);

    Task<ProfileResponse?> UpdateAsync(ProfileRequest command, Guid userId, CancellationToken cancellationToken = default);
}
