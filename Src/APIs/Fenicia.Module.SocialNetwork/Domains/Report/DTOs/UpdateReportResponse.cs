using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Report.DTOs;

public record UpdateReportResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Status);