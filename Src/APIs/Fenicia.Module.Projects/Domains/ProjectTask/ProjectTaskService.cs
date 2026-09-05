using Fenicia.Common;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Enums.Project;
using Fenicia.Module.Projects.Domains.ProjectTask.DTOs;
using Fenicia.Module.Projects.Domains.ProjectTask.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectTask;

public class ProjectTaskService(IProjectTaskRepository repository) : IProjectTaskService
{
    public async Task<List<GetAllProjectTaskResponse>> GetAllAsync(
        GetAllProjectTaskQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query();
        var filteredQuery = baseQuery;
        var tasks = await filteredQuery
            .Include(pt => pt.Assignees)
            .ThenInclude(a => a.User)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);
        return
        [
            .. tasks.Select(pt => new GetAllProjectTaskResponse(
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
                ]))
        ];
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
                ])
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
            created.CompanyId);
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
            CompanyId = companyId
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
                updated.CompanyId);
    }

    public async Task DeleteAsync(DeleteProjectTaskCommand command, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(command.Id, cancellationToken);
    }
}