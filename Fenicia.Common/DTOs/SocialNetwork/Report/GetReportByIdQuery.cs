using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Report;

public record GetReportByIdQuery([Required] Guid Id);