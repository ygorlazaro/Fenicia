using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.SocialNetwork;

namespace Fenicia.Common.Data.Models.SocialNetwork;

[Table("reports", Schema = "social_network")]
public class ReportModel : BaseModel
{
    [Required]
    public Guid ReporterId { get; init; }

    [Required]
    public Guid TargetId { get; init; }

    [Required]
    [MaxLength(32)]
    public string TargetType { get; init; } = string.Empty;

    [Required]
    [MaxLength(64)]
    public string Reason { get; init; } = string.Empty;

    [MaxLength(512)]
    public string? Description { get; init; }

    [Required]
    public EnumReportStatus Status { get; set; } = EnumReportStatus.Pending;

    [ForeignKey(nameof(ReporterId))]
    public UserModel Reporter { get; init; } = default!;

    public DateTime ReportDate { get; init; } = DateTime.UtcNow;
}