using Fenicia.Common;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Module.Projects.Domains.ProjectComment.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectComment;

public class ProjectCommentService(ProjectCommentRepository repository)
{
    public async Task<List<GetAllProjectCommentResponse>> GetAllAsync(GetAllProjectCommentQuery query, CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query();
        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);
        var comments = await filteredQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(cancellationToken);
        return [.. comments.Select(pc => new GetAllProjectCommentResponse(pc.Id, pc.TaskId, pc.UserId, pc.Content, pc.CompanyId))];
    }

    public async Task<GetProjectCommentByIdResponse?> GetByIdAsync(GetProjectCommentByIdQuery query, CancellationToken cancellationToken = default)
    {
        var projectComment = await repository.GetByIdAsync(query.Id, cancellationToken);

        return projectComment switch
        {
            null => null,
            _ => new GetProjectCommentByIdResponse(projectComment.Id, projectComment.TaskId, projectComment.UserId, projectComment.Content, projectComment.CompanyId)
        };
    }

    public async Task<AddProjectCommentResponse> AddAsync(AddProjectCommentCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        var projectComment = new ProjectCommentModel
        {
            Id = command.Id,
            TaskId = command.TaskId,
            UserId = command.UserId,
            Content = command.Content,
            CompanyId = companyId
        };

        var created = await repository.InsertAsync(projectComment, cancellationToken);
        return new AddProjectCommentResponse(created.Id, created.TaskId, created.UserId, created.Content, created.CompanyId);
    }

    public async Task<UpdateProjectCommentResponse?> UpdateAsync(UpdateProjectCommentCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Content = command.Content;
        existing.CompanyId = companyId;

        var updated = await repository.UpdateAsync(command.Id, existing, cancellationToken);
        return updated is null ? null : new UpdateProjectCommentResponse(updated.Id, updated.TaskId, updated.UserId, updated.Content, updated.CompanyId);
    }

    public async Task DeleteAsync(DeleteProjectCommentCommand command, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(command.Id, cancellationToken);
    }
}
