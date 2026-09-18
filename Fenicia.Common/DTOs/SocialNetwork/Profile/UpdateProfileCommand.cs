using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Profile;

public class UpdateProfileCommand()
{
    public UpdateProfileCommand(
        Guid id,
        string? userName,
        string? bio,
        Guid? uploadId,
        string? website,
        string? location,
        string? phone,
        DateTime? birthDate)
        : this()
    {
        Id = id;
        UserName = userName;
        Bio = bio;
        UploadId = uploadId;
        Website = website;
        Location = location;
        Phone = phone;
        BirthDate = birthDate;
    }

    [Required]
    public Guid Id { get; set; }

    [MaxLength(64)]
    public string? UserName { get; set; }

    [MaxLength(200)]
    public string? Bio { get; set; }

    public Guid? UploadId { get; set; }

    [MaxLength(200)]
    public string? Website { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }

    [MaxLength(200)]
    public string? Phone { get; set; }

    public DateTime? BirthDate { get; set; }
}
