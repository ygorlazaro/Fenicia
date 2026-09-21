using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.SocialNetwork.Profile;
using Fenicia.Module.SocialNetwork.Domains.Profile.Interfaces;

namespace Fenicia.Module.SocialNetwork.Domains.Profile;

public sealed class ProfileService(IProfileRepository repository) : IProfileService
{
    public async Task<ProfileResponse?> GetByIdAsync(
        GetProfileByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var profile = await repository.GetByIdAsync(query.Id, cancellationToken);

        return profile is null
            ? null
            : MapToResponse(profile);
    }

    public async Task<ProfileResponse?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var profile = await repository.GetByUserIdAsync(userId, cancellationToken);

        return profile is null
            ? null
            : MapToResponse(profile);
    }

    public async Task<ProfileResponse> CreateAsync(
        ProfileRequest command,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByUserIdAsync(userId, cancellationToken);
        if (existing is not null)
        {
            return MapToResponse(existing);
        }

        var model = new ProfileModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            UserName = command.UserName,
            Bio = command.Bio,
            UploadId = command.UploadId,
            Website = command.Website,
            Location = command.Location,
            Phone = command.Phone,
            BirthDate = command.BirthDate
        };

        var created = await repository.InsertAsync(model, cancellationToken);

        return MapToResponse(created);
    }

    public async Task<ProfileResponse?> UpdateAsync(
        ProfileRequest command,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var profile = await repository.GetByIdAsync(command.Id!.Value, cancellationToken);

        if (profile is null || profile.UserId != userId)
        {
            return null;
        }

        profile.UserName = command.UserName;
        profile.Bio = command.Bio;
        profile.UploadId = command.UploadId;
        profile.Website = command.Website;
        profile.Location = command.Location;
        profile.Phone = command.Phone;
        profile.BirthDate = command.BirthDate;

        await repository.UpdateAsync(command.Id.Value, profile, cancellationToken);

        var reloaded = await repository.GetByIdAsync(command.Id.Value, cancellationToken);

        return MapToResponse(reloaded!);
    }

    private static ProfileResponse MapToResponse(ProfileModel profile)
    {
        return new ProfileResponse(
            profile.Id,
            profile.UserId,
            profile.UserName,
            profile.Bio,
            profile.Upload?.Url,
            profile.UploadId,
            profile.Website,
            profile.Location,
            profile.Phone,
            profile.BirthDate);
    }
}
