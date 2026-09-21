using Fenicia.Common.DTOs.SocialNetwork.Report;

namespace Fenicia.Module.SocialNetwork.Domains.Report.Interfaces;

public interface IReportService
{
    Task<AddReportResponse> AddAsync(AddReportCommand command, Guid reporterId, CancellationToken cancellationToken = default);
    Task<UpdateReportResponse?> UpdateStatusAsync(UpdateReportStatusCommand command, CancellationToken cancellationToken = default);
    Task<List<GetAllReportResponse>> GetAllAsync(GetAllReportQuery query, CancellationToken cancellationToken = default);
    Task<GetReportByIdResponse?> GetByIdAsync(GetReportByIdQuery query, CancellationToken cancellationToken = default);
}