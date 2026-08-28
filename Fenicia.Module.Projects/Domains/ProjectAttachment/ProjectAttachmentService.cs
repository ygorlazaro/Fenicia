using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectAttachment.DTOs;
using Fenicia.Common.Data.Models.ProjectModels;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectAttachment;

public class ProjectAttachmentService(DefaultContext db)
{
    public async Task<List<GetAllProjectAttachmentResponse>> GetAllAsync(GetAllProjectAttachmentQuery query, CancellationToken ct)
    {
        return await db.ProjectAttachments.Select(p => new GetAllProjectAttachmentResponse(p.Id, p.TaskId, p.FileName, p.FileUrl, p.FileSize, p.UploadedBy, p.CompanyId)).Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(ct);
    }

    public async Task<GetProjectAttachmentByIdResponse?> GetByIdAsync(GetProjectAttachmentByIdQuery query, CancellationToken ct)
    {
        var projectAttachment = await db.ProjectAttachments.FirstOrDefaultAsync(p => p.Id == query.Id, ct);

        return projectAttachment switch
        {
            null => null,
            _ => new GetProjectAttachmentByIdResponse(projectAttachment.Id, projectAttachment.TaskId, projectAttachment.FileName, projectAttachment.FileUrl, projectAttachment.FileSize, projectAttachment.UploadedBy, projectAttachment.CompanyId)
        };
    }

    public async Task<AddProjectAttachmentResponse> AddAsync(AddProjectAttachmentCommand command, CancellationToken ct)
    {
        var projectAttachment = new AttachmentModel
        {
            Id = command.Id,
            TaskId = command.TaskId,
            FileName = command.FileName,
            FileUrl = command.FileUrl,
            FileSize = command.FileSize,
            UploadedBy = command.UploadedBy,
            ContentType = command.ContentType
        };

        db.ProjectAttachments.Add(projectAttachment);

        await db.SaveChangesAsync(ct);

        return new AddProjectAttachmentResponse(projectAttachment.Id, projectAttachment.TaskId, projectAttachment.FileName, projectAttachment.FileUrl, projectAttachment.FileSize, projectAttachment.UploadedBy, projectAttachment.CompanyId);
    }

    public async Task<UpdateProjectAttachmentResponse?> UpdateAsync(UpdateProjectAttachmentCommand command, CancellationToken ct)
    {
        var projectAttachment = await db.ProjectAttachments.FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (projectAttachment is null)
        {
            return null;
        }

        projectAttachment.TaskId = command.TaskId;
        projectAttachment.FileName = command.FileName;
        projectAttachment.FileUrl = command.FileUrl;
        projectAttachment.FileSize = command.FileSize;
        projectAttachment.UploadedBy = command.UploadedBy;

        db.ProjectAttachments.Update(projectAttachment);

        await db.SaveChangesAsync(ct);

        return new UpdateProjectAttachmentResponse(projectAttachment.Id, projectAttachment.TaskId, projectAttachment.FileName, projectAttachment.FileUrl, projectAttachment.FileSize, projectAttachment.UploadedBy, projectAttachment.CompanyId);
    }

    public async Task DeleteAsync(DeleteProjectAttachmentCommand command, CancellationToken ct)
    {
        var projectAttachment = await db.ProjectAttachments.FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (projectAttachment is null)
        {
            return;
        }

        projectAttachment.Deleted = DateTime.UtcNow;

        db.ProjectAttachments.Update(projectAttachment);

        await db.SaveChangesAsync(ct);
    }
}
