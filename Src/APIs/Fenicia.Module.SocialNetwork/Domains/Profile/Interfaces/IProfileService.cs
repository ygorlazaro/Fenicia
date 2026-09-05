using Fenicia.Module.SocialNetwork.Domains.Profile.DTOs;

namespace Fenicia.Module.SocialNetwork.Domains.Profile.Interfaces;

public interface IProfileService
{
    Task<GetProfileByIdResponse?> GetByIdAsync(GetProfileByIdQuery query, CancellationToken cancellationToken = default);

    Task<GetProfileByIdResponse?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<AddProfileResponse> CreateAsync(AddProfileCommand command, Guid userId, CancellationToken cancellationToken = default);

    Task<UpdateProfileResponse?> UpdateAsync(UpdateProfileCommand command, Guid userId, CancellationToken cancellationToken = default);
}
