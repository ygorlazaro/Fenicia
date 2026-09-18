using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Report;

public class GetReportByIdQuery()
{
    [Required] public Guid Id { get; set; }

    public GetReportByIdQuery(Guid Id)
        : this()
    {
        this.Id = Id;
    }
}
