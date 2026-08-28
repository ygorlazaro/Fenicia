using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Enums.Project;
using Fenicia.Module.Projects.Domains.ProjectTask.DTOs;
using Fenicia.Module.Projects.Domains.ProjectTask;

namespace Fenicia.Module.Projects.Domains.ProjectTask;

public class ProjectTaskService(ProjectTaskRepository repository)
{
    public async Task<List<GetAllProjectTaskResponse>> GetAllAsync(GetAllProjectTaskQuery query, CancellationToken ct)
    {
        var tasks = await repository.GetAllAsync(query.Page, query.PerPage, ct);
        return tasks.Select(pt => new GetAllProjectTaskResponse(pt.Id, pt.ProjectId, pt.StatusId, pt.Title, pt.Description, pt.Priority.ToString(), pt.Type.ToString(), pt.Order, pt.EstimatePoints, pt.DueDate, pt.CreatedBy, pt.CompanyId)).ToList();
    }

    public async Task<GetProjectTaskByIdResponse?> GetByIdAsync(GetProjectTaskByIdQuery query, CancellationToken ct)
    {
        var projectTask = await repository.GetByIdWithRelationsAsync(query.Id, ct);

        return projectTask switch
        {
            null => null,
            _ => new GetProjectTaskByIdResponse(projectTask.Id, projectTask.ProjectId, projectTask.StatusId, projectTask.Title, projectTask.Description, projectTask.Priority.ToString(), projectTask.Type.ToString(), projectTask.Order, projectTask.EstimatePoints, projectTask.DueDate, projectTask.CreatedBy, projectTask.CompanyId, projectTask.Attachments.Select(a => new ProjectAttachmentResponse(a.Id, a.FileName, a.ContentType, a.Size)).ToList(),
                projectTask.Comments.Select(c => new ProjectCommentResponse(c.Id, c.Content, c.AuthorId)).ToList(), projectTask.Subtasks.Select(s => new ProjectSubtaskResponse(s.Id, s.Title, s.IsCompleted, s.Order, s.DueDate)).ToList(), projectTask.Assignees.Select(a => new ProjectTaskAssigneeResponse(a.Id, a.UserId, a.User.Name, a.User.Email)).ToList())
        };
    }

    public async Task<AddProjectTaskResponse> AddAsync(AddProjectTaskCommand command, Guid companyId, CancellationToken ct)
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

        var created = await repository.InsertAsync(projectTask, ct);
        return new AddProjectTaskResponse(created.Id, created.ProjectId, created.StatusId, created.Title, created.Description, created.Priority.ToString(), created.Type.ToString(), created.Order, created.EstimatePoints, created.DueDate, created.CreatedBy, created.CompanyId);
    }

    public async Task<UpdateProjectTaskResponse?> UpdateAsync(UpdateProjectTaskCommand command, Guid companyId, CancellationToken ct)
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

        var updated = await repository.UpdateAsync(command.Id, projectTask, ct);
        return updated is null ? null : new UpdateProjectTaskResponse(updated.Id, updated.ProjectId, updated.StatusId, updated.Title, updated.Description, updated.Priority.ToString(), updated.Type.ToString(), updated.Order, updated.EstimatePoints, updated.DueDate, updated.CreatedBy, updated.CompanyId);
    }

    public async Task DeleteAsync(DeleteProjectTaskCommand command, CancellationToken ct)
    {
        await repository.DeleteAsync(command.Id, ct);
    }
}
