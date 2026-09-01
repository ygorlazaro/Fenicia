using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.SocialNetwork;

namespace Fenicia.Common.Data.Models.SocialNetworkModels;

[Table("reports", Schema = "social_network")]
public class ReportModel : BaseModel
{
    [Required]
    public Guid ReporterId { get; set; }

    [Required]
    public Guid TargetId { get; set; }

    [Required]
    [MaxLength(32)]
    public string TargetType { get; set; } = string.Empty;

    [Required]
    [MaxLength(64)]
    public string Reason { get; set; } = string.Empty;

    [MaxLength(512)]
    public string? Description { get; set; }

    [Required]
    public EnumReportStatus Status { get; set; } = EnumReportStatus.Pending;

    [ForeignKey(nameof(ReporterId))]
    public UserModel Reporter { get; set; } = null!;

    public DateTime ReportDate { get; set; } = DateTime.UtcNow;
}
