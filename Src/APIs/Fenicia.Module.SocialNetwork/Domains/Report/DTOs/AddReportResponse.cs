namespace Fenicia.Module.SocialNetwork.Domains.Report.DTOs;

public record AddReportResponse(Guid Id, Guid ReporterId, Guid TargetId, string TargetType, string Reason, string? Description, string Status, DateTime ReportDate);
