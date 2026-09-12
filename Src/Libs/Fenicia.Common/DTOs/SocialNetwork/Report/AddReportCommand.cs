using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Report;

public record AddReportCommand(
    [Required] Guid Id,
    [Required] Guid TargetId,
    [Required] [MaxLength(200)] string TargetType,
    [Required] [MaxLength(200)] string Reason,
    [MaxLength(200)] string? Description);