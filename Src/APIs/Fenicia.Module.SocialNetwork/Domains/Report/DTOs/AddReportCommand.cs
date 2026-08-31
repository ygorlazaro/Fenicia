namespace Fenicia.Module.SocialNetwork.Domains.Report.DTOs;

public record AddReportCommand(Guid Id, Guid TargetId, string TargetType, string Reason, string? Description);
