using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.Auth;

[Table("profiles", Schema = "auth")]
public class ProfileModel : BaseModel
{
    [Required]
    public Guid UserId { get; init; }

    public UserModel User { get; init; } = default!;

    [MaxLength(64)]
    public string? UserName { get; set; }

    [MaxLength(160)]
    public string? Bio { get; set; }

    public Guid? UploadId { get; set; }

    [ForeignKey(nameof(UploadId))]
    public UploadModel? Upload { get; init; }

    [MaxLength(120)]
    public string? Website { get; set; }

    [MaxLength(64)]
    public string? Location { get; set; }

    [MaxLength(24)]
    public string? Phone { get; set; }

    public DateTime? BirthDate { get; set; }
}