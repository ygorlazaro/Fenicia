using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Report;

public class UpdateReportStatusCommand()
{
    [Required] public Guid Id { get; set; }
    [Required] [MaxLength(200)] public string Status { get; set; } = string.Empty;

    public UpdateReportStatusCommand(Guid Id, string Status)
        : this()
    {
        this.Id = Id;
        this.Status = Status;
    }
}
