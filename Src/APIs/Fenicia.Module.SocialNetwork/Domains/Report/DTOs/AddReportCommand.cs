using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Report.DTOs;

public record AddReportCommand(
    [Required] Guid Id,
    [Required] Guid TargetId,
    [Required][MaxLength(200)] string TargetType,
    [Required][MaxLength(200)] string Reason,
    [MaxLength(200)] string? Description);
