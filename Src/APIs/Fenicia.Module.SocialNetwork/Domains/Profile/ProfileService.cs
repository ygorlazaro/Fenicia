using Fenicia.Module.SocialNetwork.Domains.Profile.DTOs;

namespace Fenicia.Module.SocialNetwork.Domains.Profile;

public sealed class ProfileService(IProfileRepository profileRepository)
{
    public ProfileService()
        : this(null!)
    {
    }

    public async Task<GetProfileByIdResponse?> GetByIdAsync(
        GetProfileByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var profile = await profileRepository.GetByIdAsync(query.Id, cancellationToken);

        return profile is null
            ? null
            : new GetProfileByIdResponse(
                profile.Id,
                profile.UserId,
                profile.Bio,
                profile.ImageUrl,
                profile.Website,
                profile.Location,
                profile.Phone,
                profile.BirthDate);
    }

    public async Task<UpdateProfileResponse?> UpdateAsync(
        UpdateProfileCommand command,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var profile = await profileRepository.GetByIdAsync(command.Id, cancellationToken);

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

        await profileRepository.UpdateAsync(command.Id, profile, cancellationToken);

        return new UpdateProfileResponse(
            profile.Id,
            profile.UserId,
            profile.Bio,
            profile.ImageUrl,
            profile.Website,
            profile.Location,
            profile.Phone,
            profile.BirthDate);
    }
}