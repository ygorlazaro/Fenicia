using Fenicia.Module.SocialNetwork.Domains.Profile.DTOs;

namespace Fenicia.Module.SocialNetwork.Domains.Profile;

public class ProfileService(IProfileRepository profileRepository)
{
    public ProfileService()
        : this(null!)
    {
    }

    public virtual async Task<GetProfileByIdResponse?> GetByIdAsync(GetProfileByIdQuery query, CancellationToken ct)
    {
        var profile = await profileRepository.GetByIdAsync(query.Id, ct);

        return profile is null ? null : new GetProfileByIdResponse(profile.Id, profile.UserId, profile.Bio, profile.ImageUrl, profile.Website, profile.Location, profile.Phone, profile.BirthDate);
    }

    public virtual async Task<UpdateProfileResponse?> UpdateAsync(UpdateProfileCommand command, Guid userId, CancellationToken ct)
    {
        var profile = await profileRepository.GetByIdAsync(command.Id, ct);

        if (profile is null)
        {
            return null;
        }

        profile.Bio = command.Bio;
        profile.ImageUrl = command.ImageUrl;
        profile.Website = command.Website;
        profile.Location = command.Location;
        profile.Phone = command.Phone;
        profile.BirthDate = command.BirthDate;

        await profileRepository.UpdateAsync(command.Id, profile, ct);

        return new UpdateProfileResponse(profile.Id, profile.UserId, profile.Bio, profile.ImageUrl, profile.Website, profile.Location, profile.Phone, profile.BirthDate);
    }
}
