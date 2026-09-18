using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.DTOs.Project.ProjectAttachment;
using Fenicia.Module.Projects.Domains.ProjectAttachment.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectAttachment;

public class ProjectAttachmentService(IRepository<AttachmentModel> repository, ProjectAttachmentMapper mapper) : IProjectAttachmentService
{
    public async Task<List<GetAllProjectAttachmentResponse>> GetAllAsync(
        GetAllProjectAttachmentQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query();
        var attachments = await baseQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage)
            .ToListAsync(cancellationToken);
        return [.. attachments.Select(mapper.MapToGetAllProjectAttachmentResponse)];
    }

    public async Task<GetProjectAttachmentByIdResponse?> GetByIdAsync(
        GetProjectAttachmentByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var projectAttachment = await repository.GetByIdAsync(query.Id, cancellationToken);

        return projectAttachment is null
            ? null
            : mapper.MapToGetProjectAttachmentByIdResponse(projectAttachment);
    }

    public async Task<AddProjectAttachmentResponse> AddAsync(
        AddProjectAttachmentCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
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
        return mapper.MapToAddProjectAttachmentResponse(created);
    }

    public async Task<UpdateProjectAttachmentResponse?> UpdateAsync(
        UpdateProjectAttachmentCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
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
        return updated is null
            ? null
            : mapper.MapToUpdateProjectAttachmentResponse(updated);
    }

    public async Task DeleteAsync(DeleteProjectAttachmentCommand command, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(command.Id, cancellationToken);
    }
}