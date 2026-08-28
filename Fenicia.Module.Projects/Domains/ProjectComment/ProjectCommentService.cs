using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Module.Projects.Domains.ProjectComment.DTOs;

namespace Fenicia.Module.Projects.Domains.ProjectComment;

public class ProjectCommentService(ProjectCommentRepository repository)
{
    public async Task<List<GetAllProjectCommentResponse>> GetAllAsync(GetAllProjectCommentQuery query, CancellationToken ct)
    {
        var comments = await repository.GetAllAsync(query.Page, query.PerPage, ct);
        return comments.Select(pc => new GetAllProjectCommentResponse(pc.Id, pc.TaskId, pc.UserId, pc.Content, pc.CompanyId)).ToList();
    }

    public async Task<GetProjectCommentByIdResponse?> GetByIdAsync(GetProjectCommentByIdQuery query, CancellationToken ct)
    {
        var projectComment = await repository.GetByIdAsync(query.Id, ct);

        return projectComment switch
        {
            null => null,
            _ => new GetProjectCommentByIdResponse(projectComment.Id, projectComment.TaskId, projectComment.UserId, projectComment.Content, projectComment.CompanyId)
        };
    }

    public async Task<AddProjectCommentResponse> AddAsync(AddProjectCommentCommand command, Guid companyId, CancellationToken ct)
    {
        var projectComment = new ProjectCommentModel
        {
            Id = command.Id,
            TaskId = command.TaskId,
            UserId = command.UserId,
            Content = command.Content,
            CompanyId = companyId
        };

        var created = await repository.InsertAsync(projectComment, ct);
        return new AddProjectCommentResponse(created.Id, created.TaskId, created.UserId, created.Content, created.CompanyId);
    }

    public async Task<UpdateProjectCommentResponse?> UpdateAsync(UpdateProjectCommentCommand command, Guid companyId, CancellationToken ct)
    {
        var existing = await repository.GetByIdAsync(command.Id, ct);
        if (existing is null)
        {
            return null;
        }

        existing.Content = command.Content;
        existing.CompanyId = companyId;

        var updated = await repository.UpdateAsync(command.Id, existing, ct);
        return updated is null ? null : new UpdateProjectCommentResponse(updated.Id, updated.TaskId, updated.UserId, updated.Content, updated.CompanyId);
    }

    public async Task DeleteAsync(DeleteProjectCommentCommand command, CancellationToken ct)
    {
        await repository.DeleteAsync(command.Id, ct);
    }
}
