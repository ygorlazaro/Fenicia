using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Report;

public record UpdateReportResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Status);