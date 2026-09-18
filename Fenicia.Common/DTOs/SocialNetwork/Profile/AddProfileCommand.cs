using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Profile;

public class AddProfileCommand()
{
    public AddProfileCommand(
        string? userName,
        string? bio,
        Guid? uploadId,
        string? website,
        string? location,
        string? phone,
        DateTime? birthDate)
        : this()
    {
        UserName = userName;
        Bio = bio;
        UploadId = uploadId;
        Website = website;
        Location = location;
        Phone = phone;
        BirthDate = birthDate;
    }

    [MaxLength(64)]
    public string? UserName { get; set; }

    [MaxLength(160)]
    public string? Bio { get; set; }

    public Guid? UploadId { get; set; }

    [MaxLength(120)]
    public string? Website { get; set; }

    [MaxLength(64)]
    public string? Location { get; set; }

    [MaxLength(24)]
    public string? Phone { get; set; }

    public DateTime? BirthDate { get; set; }
}
