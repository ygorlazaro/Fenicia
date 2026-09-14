using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.DTOs.Project.ProjectTask;
using Fenicia.Common.Enums.Project;
using Fenicia.Module.Projects.Domains.ProjectTask.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectTask;

public class ProjectTaskService(IProjectTaskRepository repository) : IProjectTaskService
{
    public async Task<List<GetAllProjectTaskResponse>> GetAllAsync(
        GetAllProjectTaskQuery query,
        CancellationToken cancellationToken = default)
    {
        var filteredQuery = BuildFilteredQuery(query);
        var tasks = await GetTasksWithRelationsAsync(filteredQuery, query, cancellationToken);
        return [.. tasks.Select(MapToGetAllResponse)];
    }

    public async Task<GetProjectTaskByIdResponse?> GetByIdAsync(
        GetProjectTaskByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var projectTask = await repository.GetByIdWithRelationsAsync(query.Id, cancellationToken);

        return projectTask switch
        {
            null => null,
            _ => new GetProjectTaskByIdResponse(
                projectTask.Id,
                projectTask.ProjectId,
                projectTask.StatusId,
                projectTask.Title,
                projectTask.Description,
                projectTask.Priority.ToString(),
                projectTask.Type.ToString(),
                projectTask.Order,
                projectTask.EstimatePoints,
                projectTask.DueDate,
                projectTask.CreatedBy,
                projectTask.CompanyId,
                [
                    .. projectTask.Attachments.Select(a => new ProjectAttachmentResponse(
                        a.Id,
                        a.FileName,
                        a.ContentType ?? string.Empty,
                        a.Size))
                ],
                [.. projectTask.Comments.Select(c => new ProjectCommentResponse(c.Id, c.Content, c.AuthorId))],
                [
                    .. projectTask.Subtasks.Select(s => new ProjectSubtaskResponse(
                        s.Id,
                        s.Title,
                        s.IsCompleted,
                        s.Order,
                        s.DueDate))
                ],
                [
                    .. projectTask.Assignees.Select(a =>
                        new ProjectTaskAssigneeResponse(a.Id, a.UserId, a.User.Name, a.User.Email))
                ],
                projectTask.SprintId,
                projectTask.SprintModel?.Name)
        };
    }

    public async Task<AddProjectTaskResponse> AddAsync(
        AddProjectTaskCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var projectTask = new ProjectTaskModel
        {
            Id = command.Id,
            ProjectId = command.ProjectId,
            StatusId = command.StatusId,
            Title = command.Title,
            Description = command.Description,
            Priority = Enum.Parse<EnumTaskPriority>(command.Priority, true),
            Type = Enum.Parse<EnumTaskType>(command.Type, true),
            Order = command.Order,
            EstimatePoints = command.EstimatePoints,
            DueDate = command.DueDate,
            CreatedBy = command.CreatedBy,
            SprintId = command.SprintId,
            CompanyId = companyId
        };

        var created = await repository.InsertAsync(projectTask, cancellationToken);
        return new AddProjectTaskResponse(
            created.Id,
            created.ProjectId,
            created.StatusId,
            created.Title,
            created.Description,
            created.Priority.ToString(),
            created.Type.ToString(),
            created.Order,
            created.EstimatePoints,
            created.DueDate,
            created.CreatedBy,
            created.CompanyId,
            created.SprintId,
            created.SprintModel?.Name);
    }

    public async Task<UpdateProjectTaskResponse?> UpdateAsync(
        UpdateProjectTaskCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var projectTask = new ProjectTaskModel
        {
            Id = command.Id,
            ProjectId = command.ProjectId,
            StatusId = command.StatusId,
            Title = command.Title,
            Description = command.Description,
            Priority = Enum.Parse<EnumTaskPriority>(command.Priority, true),
            Type = Enum.Parse<EnumTaskType>(command.Type, true),
            Order = command.Order,
            EstimatePoints = command.EstimatePoints,
            DueDate = command.DueDate,
            CreatedBy = command.CreatedBy,
            SprintId = command.SprintId
        };

        var updated = await repository.UpdateAsync(command.Id, projectTask, cancellationToken);
        return updated is null
            ? null
            : new UpdateProjectTaskResponse(
                updated.Id,
                updated.ProjectId,
                updated.StatusId,
                updated.Title,
                updated.Description,
                updated.Priority.ToString(),
                updated.Type.ToString(),
                updated.Order,
                updated.EstimatePoints,
                updated.DueDate,
                updated.CreatedBy,
                updated.CompanyId,
                updated.SprintId,
                updated.SprintModel?.Name);
    }

    public async Task DeleteAsync(DeleteProjectTaskCommand command, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(command.Id, cancellationToken);
    }

    private static IQueryable<ProjectTaskModel> ApplyStatusIdFilter(IQueryable<ProjectTaskModel> query, GetAllProjectTaskQuery q)
    {
        return q.StatusId.HasValue ? query.Where(t => t.StatusId == q.StatusId.Value) : query;
    }

    private static IQueryable<ProjectTaskModel> ApplyCreatedByFilter(IQueryable<ProjectTaskModel> query, GetAllProjectTaskQuery q)
    {
        return q.CreatedBy.HasValue ? query.Where(t => t.CreatedBy == q.CreatedBy.Value) : query;
    }

    private static IQueryable<ProjectTaskModel> ApplyAssigneeIdFilter(IQueryable<ProjectTaskModel> query, GetAllProjectTaskQuery q)
    {
        return q.AssigneeId.HasValue ? query.Where(t => t.Assignees.Any(a => a.UserId == q.AssigneeId.Value)) : query;
    }

    private static IQueryable<ProjectTaskModel> ApplyDueDateRangeFilter(IQueryable<ProjectTaskModel> query, GetAllProjectTaskQuery q)
    {
        if (q.DueFrom.HasValue)
        {
            var from = q.DueFrom.Value;
            query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value >= from);
        }

        if (!q.DueTo.HasValue)
        {
            return query;
        }

        var to = q.DueTo.Value;
        query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value <= to);

        return query;
    }

    private static IQueryable<ProjectTaskModel> ApplyTypeFilter(IQueryable<ProjectTaskModel> query, GetAllProjectTaskQuery q)
    {
        return !string.IsNullOrWhiteSpace(q.Type) ? query.Where(t => t.Type.ToString() == q.Type) : query;
    }

    private static IQueryable<ProjectTaskModel> ApplyPriorityFilter(IQueryable<ProjectTaskModel> query, GetAllProjectTaskQuery q)
    {
        return !string.IsNullOrWhiteSpace(q.Priority) ? query.Where(t => t.Priority.ToString() == q.Priority) : query;
    }

    private static IQueryable<ProjectTaskModel> ApplySprintIdFilter(IQueryable<ProjectTaskModel> query, GetAllProjectTaskQuery q)
    {
        return q.SprintId.HasValue ? query.Where(t => t.SprintId == q.SprintId.Value) : query;
    }

    private static IQueryable<ProjectTaskModel> ApplyWithoutSprintFilter(IQueryable<ProjectTaskModel> query, GetAllProjectTaskQuery q)
    {
        return q.WithoutSprint == true ? query.Where(t => !t.SprintId.HasValue) : query;
    }

    private static GetAllProjectTaskResponse MapToGetAllResponse(ProjectTaskModel pt)
    {
        return new GetAllProjectTaskResponse(
            pt.Id,
            pt.ProjectId,
            pt.StatusId,
            pt.Title,
            pt.Description,
            pt.Priority.ToString(),
            pt.Type.ToString(),
            pt.Order,
            pt.EstimatePoints,
            pt.DueDate,
            pt.CreatedBy,
            pt.CompanyId,
            [
                .. pt.Assignees.Select(a =>
                    new ProjectTaskAssigneeResponse(a.Id, a.UserId, a.User.Name, a.User.Email))
            ],
            pt.Comments.Count,
            pt.Subtasks.Count,
            pt.Subtasks.Count(s => s.IsCompleted),
            pt.SprintId,
            pt.SprintModel?.Name);
    }

    private static Task<List<ProjectTaskModel>> GetTasksWithRelationsAsync(
        IQueryable<ProjectTaskModel> filteredQuery,
        GetAllProjectTaskQuery query,
        CancellationToken cancellationToken)
    {
        return filteredQuery
            .Include(pt => pt.Assignees)
            .ThenInclude(a => a.User)
            .Include(pt => pt.Comments)
            .Include(pt => pt.Subtasks)
            .Include(pt => pt.SprintModel)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<ProjectTaskModel> BuildFilteredQuery(GetAllProjectTaskQuery query)
    {
        var filteredQuery = repository.Query();

        filteredQuery = ApplyStatusIdFilter(filteredQuery, query);
        filteredQuery = ApplyCreatedByFilter(filteredQuery, query);
        filteredQuery = ApplyAssigneeIdFilter(filteredQuery, query);
        filteredQuery = ApplyDueDateRangeFilter(filteredQuery, query);
        filteredQuery = ApplyTypeFilter(filteredQuery, query);
        filteredQuery = ApplyPriorityFilter(filteredQuery, query);
        filteredQuery = ApplySprintIdFilter(filteredQuery, query);
        filteredQuery = ApplyWithoutSprintFilter(filteredQuery, query);

        return filteredQuery;
    }
}