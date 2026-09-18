using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Report;

public class GetAllReportResponse()
{
    [Required] public Guid Id { get; init; }
    [Required] public Guid ReporterId { get; init; }
    [Required] public Guid TargetId { get; init; }
    [Required] [MaxLength(200)] public string TargetType { get; init; } = string.Empty;
    [Required] [MaxLength(200)] public string Reason { get; init; } = string.Empty;
    [MaxLength(200)] public string? Description { get; init; }
    [Required] [MaxLength(200)] public string Status { get; init; } = string.Empty;
    [Required] public DateTime ReportDate { get; init; }

    public GetAllReportResponse(Guid Id, Guid ReporterId, Guid TargetId, string TargetType, string Reason, string? Description, string Status, DateTime ReportDate)
        : this()
    {
        this.Id = Id;
        this.ReporterId = ReporterId;
        this.TargetId = TargetId;
        this.TargetType = TargetType;
        this.Reason = Reason;
        this.Description = Description;
        this.Status = Status;
        this.ReportDate = ReportDate;
    }
}
