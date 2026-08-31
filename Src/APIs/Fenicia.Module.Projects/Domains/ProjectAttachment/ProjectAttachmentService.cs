using Fenicia.Common;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Module.Projects.Domains.ProjectAttachment.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectAttachment;

public class ProjectAttachmentService(ProjectAttachmentRepository repository)
{
    public async Task<List<GetAllProjectAttachmentResponse>> GetAllAsync(GetAllProjectAttachmentQuery query, CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query();
        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);
        var attachments = await filteredQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(cancellationToken);
        return [.. attachments.Select(p => new GetAllProjectAttachmentResponse(p.Id, p.TaskId, p.FileName, p.FileUrl, p.FileSize, p.UploadedBy, p.CompanyId))];
    }

    public async Task<GetProjectAttachmentByIdResponse?> GetByIdAsync(GetProjectAttachmentByIdQuery query, CancellationToken cancellationToken = default)
    {
        var projectAttachment = await repository.GetByIdAsync(query.Id, cancellationToken);

        return projectAttachment switch
        {
            null => null,
            _ => new GetProjectAttachmentByIdResponse(projectAttachment.Id, projectAttachment.TaskId, projectAttachment.FileName, projectAttachment.FileUrl, projectAttachment.FileSize, projectAttachment.UploadedBy, projectAttachment.CompanyId)
        };
    }

    public async Task<AddProjectAttachmentResponse> AddAsync(AddProjectAttachmentCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        var projectAttachment = new AttachmentModel
        {
            Id = command.Id,
            TaskId = command.TaskId,
            FileName = command.FileName,
            FileUrl = command.FileUrl,
            FileSize = command.FileSize,
            UploadedBy = command.UploadedBy,
            ContentType = command.ContentType,
            CompanyId = companyId
        };

        var created = await repository.InsertAsync(projectAttachment, cancellationToken);
        return new AddProjectAttachmentResponse(created.Id, created.TaskId, created.FileName, created.FileUrl, created.FileSize, created.UploadedBy, created.CompanyId);
    }

    public async Task<UpdateProjectAttachmentResponse?> UpdateAsync(UpdateProjectAttachmentCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        var projectAttachment = new AttachmentModel
        {
            Id = command.Id,
            TaskId = command.TaskId,
            FileName = command.FileName,
            FileUrl = command.FileUrl,
            FileSize = command.FileSize,
            UploadedBy = command.UploadedBy,
            CompanyId = companyId
        };

        var updated = await repository.UpdateAsync(command.Id, projectAttachment, cancellationToken);
        return updated is null ? null : new UpdateProjectAttachmentResponse(updated.Id, updated.TaskId, updated.FileName, updated.FileUrl, updated.FileSize, updated.UploadedBy, updated.CompanyId);
    }

    public async Task DeleteAsync(DeleteProjectAttachmentCommand command, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(command.Id, cancellationToken);
    }
}
