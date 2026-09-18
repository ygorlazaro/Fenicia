using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Report;

public class UpdateReportResponse()
{
    [Required] public Guid Id { get; init; }
    [Required] [MaxLength(200)] public string Status { get; init; } = string.Empty;

    public UpdateReportResponse(Guid Id, string Status)
        : this()
    {
        this.Id = Id;
        this.Status = Status;
    }
}
