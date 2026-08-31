using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Enums.SocialNetwork;
using Fenicia.Module.SocialNetwork.Domains.Report.DTOs;

namespace Fenicia.Module.SocialNetwork.Domains.Report;

public class ReportService(ReportRepository repository)
{
    public async Task<AddReportResponse> AddAsync(AddReportCommand command, Guid reporterId, CancellationToken ct)
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

        var created = await repository.InsertAsync(model, ct);
        return new AddReportResponse(created.Id, created.ReporterId, created.TargetId, created.TargetType, created.Reason, created.Description, created.Status.ToString(), created.ReportDate);
    }

    public async Task<UpdateReportResponse?> UpdateStatusAsync(UpdateReportStatusCommand command, CancellationToken ct)
    {
        var existing = await repository.GetByIdAsync(command.Id, ct);
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
        var updated = await repository.UpdateAsync(command.Id, existing, ct);
        return updated is null ? null : new UpdateReportResponse(updated.Id, updated.Status.ToString());
    }

    public async Task<List<GetAllReportResponse>> GetAllAsync(GetAllReportQuery query, CancellationToken ct)
    {
        var reports = await repository.GetAllAsync(query.Page, query.PerPage, ct);
        return reports.Select(r => new GetAllReportResponse(r.Id, r.ReporterId, r.TargetId, r.TargetType, r.Reason, r.Description, r.Status.ToString(), r.ReportDate)).ToList();
    }

    public async Task<GetReportByIdResponse?> GetByIdAsync(GetReportByIdQuery query, CancellationToken ct)
    {
        var report = await repository.GetByIdAsync(query.Id, ct);
        return report is null ? null : new GetReportByIdResponse(report.Id, report.ReporterId, report.TargetId, report.TargetType, report.Reason, report.Description, report.Status.ToString(), report.ReportDate);
    }
}
