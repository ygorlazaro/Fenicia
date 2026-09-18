using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Profile;

public class AddProfileResponse()
{
    public AddProfileResponse(
        Guid id,
        Guid userId,
        string? userName,
        string? bio,
        string? imageUrl,
        Guid? uploadId,
        string? website,
        string? location,
        string? phone,
        DateTime? birthDate)
        : this()
    {
        Id = id;
        UserId = userId;
        UserName = userName;
        Bio = bio;
        ImageUrl = imageUrl;
        UploadId = uploadId;
        Website = website;
        Location = location;
        Phone = phone;
        BirthDate = birthDate;
    }

    [Required]
    public Guid Id { get; init; }

    [Required]
    public Guid UserId { get; init; }

    [MaxLength(64)]
    public string? UserName { get; init; }

    [MaxLength(160)]
    public string? Bio { get; init; }

    public string? ImageUrl { get; init; }

    public Guid? UploadId { get; init; }

    [MaxLength(120)]
    public string? Website { get; init; }

    [MaxLength(64)]
    public string? Location { get; init; }

    [MaxLength(24)]
    public string? Phone { get; init; }

    public DateTime? BirthDate { get; init; }
}
