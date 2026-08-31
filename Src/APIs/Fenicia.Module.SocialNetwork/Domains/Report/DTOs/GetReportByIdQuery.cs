using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Report.DTOs;

public record GetReportByIdQuery(
    [Required] Guid Id);
