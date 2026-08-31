using Fenicia.Common;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Module.Projects.Domains.ProjectSubtask.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectSubtask;

public class ProjectSubtaskService(ProjectSubtaskRepository repository)
{
    public async Task<List<GetAllProjectSubtaskResponse>> GetAllAsync(GetAllProjectSubtaskQuery query, CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query();
        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);
        var subtasks = await filteredQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(cancellationToken);
        return [.. subtasks.Select(ps => new GetAllProjectSubtaskResponse(ps.Id, ps.TaskId, ps.Title, ps.IsCompleted, ps.Order, ps.CompletedAt, ps.CompanyId))];
    }

    public async Task<GetProjectSubtaskByIdResponse?> GetByIdAsync(GetProjectSubtaskByIdQuery query, CancellationToken cancellationToken = default)
    {
        var projectSubtask = await repository.GetByIdAsync(query.Id, cancellationToken);

        return projectSubtask switch
        {
            null => null,
            _ => new GetProjectSubtaskByIdResponse(projectSubtask.Id, projectSubtask.TaskId, projectSubtask.Title, projectSubtask.IsCompleted, projectSubtask.Order, projectSubtask.CompletedAt, projectSubtask.CompanyId)
        };
    }

    public async Task<AddProjectSubtaskResponse> AddAsync(AddProjectSubtaskCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        var projectSubtask = new ProjectSubtaskModel
        {
            Id = command.Id,
            TaskId = command.TaskId,
            Title = command.Title,
            IsCompleted = command.IsCompleted,
            Order = command.Order,
            CompletedAt = command.CompletedAt,
            CompanyId = companyId
        };

        var created = await repository.InsertAsync(projectSubtask, cancellationToken);
        return new AddProjectSubtaskResponse(created.Id, created.TaskId, created.Title, created.IsCompleted, created.Order, created.CompletedAt, created.CompanyId);
    }

    public async Task<UpdateProjectSubtaskResponse?> UpdateAsync(UpdateProjectSubtaskCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        var projectSubtask = new ProjectSubtaskModel
        {
            Id = command.Id,
            TaskId = command.TaskId,
            Title = command.Title,
            IsCompleted = command.IsCompleted,
            Order = command.Order,
            CompletedAt = command.CompletedAt,
            CompanyId = companyId
        };

        var updated = await repository.UpdateAsync(command.Id, projectSubtask, cancellationToken);
        return updated is null ? null : new UpdateProjectSubtaskResponse(updated.Id, updated.TaskId, updated.Title, updated.IsCompleted, updated.Order, updated.CompletedAt, updated.CompanyId);
    }

    public async Task DeleteAsync(DeleteProjectSubtaskCommand command, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(command.Id, cancellationToken);
    }
}
