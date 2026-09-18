using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Report;

public class AddReportCommand()
{
    [Required] public Guid Id { get; set; }
    [Required] public Guid TargetId { get; set; }
    [Required] [MaxLength(200)] public string TargetType { get; set; } = string.Empty;
    [Required] [MaxLength(200)] public string Reason { get; set; } = string.Empty;
    [MaxLength(200)] public string? Description { get; set; }

    public AddReportCommand(Guid Id, Guid TargetId, string TargetType, string Reason, string? Description)
        : this()
    {
        this.Id = Id;
        this.TargetId = TargetId;
        this.TargetType = TargetType;
        this.Reason = Reason;
        this.Description = Description;
    }
}
