using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Module.SocialNetwork.Domains.Profile.DTOs;
using Fenicia.Module.SocialNetwork.Domains.Profile.Interfaces;

namespace Fenicia.Module.SocialNetwork.Domains.Profile;

public sealed class ProfileService(IProfileRepository profileRepository) : IProfileService
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
            : MapToGetResponse(profile);
    }

    public async Task<GetProfileByIdResponse?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var profile = await profileRepository.GetByUserIdAsync(userId, cancellationToken);

        return profile is null
            ? null
            : MapToGetResponse(profile);
    }

    public async Task<AddProfileResponse> CreateAsync(
        AddProfileCommand command,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var existing = await profileRepository.GetByUserIdAsync(userId, cancellationToken);
        if (existing is not null)
        {
            return MapToAddResponse(existing);
        }

        var model = new ProfileModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            UserName = command.UserName,
            Bio = command.Bio,
            ImageUrl = command.ImageUrl,
            Website = command.Website,
            Location = command.Location,
            Phone = command.Phone,
            BirthDate = command.BirthDate,
        };

        var created = await profileRepository.InsertAsync(model, cancellationToken);

        return MapToAddResponse(created);
    }

    public async Task<UpdateProfileResponse?> UpdateAsync(
        UpdateProfileCommand command,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var profile = await profileRepository.GetByIdAsync(command.Id, cancellationToken);

        if (profile is null || profile.UserId != userId)
        {
            return null;
        }

        profile.UserName = command.UserName;
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
            profile.UserName,
            profile.Bio,
            profile.ImageUrl,
            profile.Website,
            profile.Location,
            profile.Phone,
            profile.BirthDate);
    }

    private static GetProfileByIdResponse MapToGetResponse(ProfileModel profile)
    {
        return new GetProfileByIdResponse(
            profile.Id,
            profile.UserId,
            profile.UserName,
            profile.Bio,
            profile.ImageUrl,
            profile.Website,
            profile.Location,
            profile.Phone,
            profile.BirthDate);
    }

    private static AddProfileResponse MapToAddResponse(ProfileModel profile)
    {
        return new AddProfileResponse(
            profile.Id,
            profile.UserId,
            profile.UserName,
            profile.Bio,
            profile.ImageUrl,
            profile.Website,
            profile.Location,
            profile.Phone,
            profile.BirthDate);
    }
}
