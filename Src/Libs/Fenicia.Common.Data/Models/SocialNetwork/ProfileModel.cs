using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.SocialNetwork;

[Table("profiles", Schema = "social_network")]
public class ProfileModel : BaseModel
{
    [Required]
    public Guid UserId { get; init; }

    [ForeignKey(nameof(UserId))]
    public UserModel User { get; init; } = default!;

    [MaxLength(64)]
    public string? UserName { get; set; }

    [MaxLength(160)]
    public string? Bio { get; set; }

    [MaxLength(48)]
    public string? ImageUrl { get; set; }

    [MaxLength(120)]
    public string? Website { get; set; }

    [MaxLength(64)]
    public string? Location { get; set; }

    [MaxLength(24)]
    public string? Phone { get; set; }

    public DateTime? BirthDate { get; set; }
}