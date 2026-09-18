using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.DTOs.SocialNetwork.Report;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.SocialNetwork.Domains.Report;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ReportMapper
{
    public partial AddReportResponse MapToAddReportResponse(ReportModel report);

    public partial GetAllReportResponse MapToGetAllReportResponse(ReportModel report);

    public partial GetReportByIdResponse MapToGetReportByIdResponse(ReportModel report);

    public partial UpdateReportResponse MapToUpdateReportResponse(ReportModel report);
}
