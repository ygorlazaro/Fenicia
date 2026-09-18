using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.DTOs.SocialNetwork.Report;
using Fenicia.Common.Enums.SocialNetwork;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Report;

public class ReportService(ReportRepository repository, ReportMapper mapper)
{
    public async Task<AddReportResponse> AddAsync(
        AddReportCommand command,
        Guid reporterId,
        CancellationToken cancellationToken = default)
    {
        var model = new ReportModel
        {
            Id = command.Id,
            ReporterId = reporterId,
            TargetId = command.TargetId,
            TargetType = command.TargetType,
            Reason = command.Reason,
            Description = command.Description,
            Status = EnumReportStatus.Pending,
            ReportDate = DateTime.UtcNow
        };

        var created = await repository.InsertAsync(model, cancellationToken);
        return mapper.MapToAddReportResponse(created);
    }

    public async Task<UpdateReportResponse?> UpdateStatusAsync(
        UpdateReportStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        var newStatus = Enum.Parse<EnumReportStatus>(command.Status, true);
        if (newStatus != EnumReportStatus.Approved && newStatus != EnumReportStatus.Denied)
        {
            throw new ArgumentException("Status must be Approved or Denied");
        }

        existing.Status = newStatus;
        var updated = await repository.UpdateAsync(command.Id, existing, cancellationToken);
        return updated is null ? null : mapper.MapToUpdateReportResponse(updated);
    }

    public async Task<List<GetAllReportResponse>> GetAllAsync(
        GetAllReportQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query();
        var reports = await baseQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage)
            .ToListAsync(cancellationToken);
        return [.. reports.Select(mapper.MapToGetAllReportResponse)];
    }

    public async Task<GetReportByIdResponse?> GetByIdAsync(
        GetReportByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var report = await repository.GetByIdAsync(query.Id, cancellationToken);
        return report is null
            ? null
            : mapper.MapToGetReportByIdResponse(report);
    }
}
