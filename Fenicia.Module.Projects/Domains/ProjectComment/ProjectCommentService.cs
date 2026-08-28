using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectComment.DTOs;
using Fenicia.Common.Data.Models.ProjectModels;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectComment;

public class ProjectCommentService(DefaultContext db)
{
    public async Task<List<GetAllProjectCommentResponse>> GetAllAsync(GetAllProjectCommentQuery query, CancellationToken ct)
    {
        return await db.ProjectComments.Select(pc => new GetAllProjectCommentResponse(pc.Id, pc.TaskId, pc.UserId, pc.Content, pc.CompanyId)).Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(ct);
    }

    public async Task<GetProjectCommentByIdResponse?> GetByIdAsync(GetProjectCommentByIdQuery query, CancellationToken ct)
    {
        var projectComment = await db.ProjectComments.FirstOrDefaultAsync(pc => pc.Id == query.Id, ct);

        return projectComment switch
        {
            null => null,
            _ => new GetProjectCommentByIdResponse(projectComment.Id, projectComment.TaskId, projectComment.UserId, projectComment.Content, projectComment.CompanyId)
        };
    }

    public async Task<AddProjectCommentResponse> AddAsync(AddProjectCommentCommand command, CancellationToken ct)
    {
        var projectComment = new ProjectCommentModel
        {
            Id = command.Id,
            TaskId = command.TaskId,
            UserId = command.UserId,
            Content = command.Content
        };

        db.ProjectComments.Add(projectComment);

        await db.SaveChangesAsync(ct);

        return new AddProjectCommentResponse(projectComment.Id, projectComment.TaskId, projectComment.UserId, projectComment.Content, projectComment.CompanyId);
    }

    public async Task<UpdateProjectCommentResponse?> UpdateAsync(UpdateProjectCommentCommand command, CancellationToken ct)
    {
        var projectComment = await db.ProjectComments.FirstOrDefaultAsync(pc => pc.Id == command.Id, ct);

        if (projectComment is null)
        {
            return null;
        }

        projectComment.Content = command.Content;

        db.ProjectComments.Update(projectComment);

        await db.SaveChangesAsync(ct);

        return new UpdateProjectCommentResponse(projectComment.Id, projectComment.TaskId, projectComment.UserId, projectComment.Content, projectComment.CompanyId);
    }

    public async Task DeleteAsync(DeleteProjectCommentCommand command, CancellationToken ct)
    {
        var projectComment = await db.ProjectComments.FirstOrDefaultAsync(pc => pc.Id == command.Id, ct);

        if (projectComment is null)
        {
            return;
        }

        projectComment.Deleted = DateTime.UtcNow;

        db.ProjectComments.Update(projectComment);

        await db.SaveChangesAsync(ct);
    }
}
