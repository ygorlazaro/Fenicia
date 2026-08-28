using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectTask.DTOs;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Enums.Project;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectTask;

public class ProjectTaskService(DefaultContext db)
{
    public async Task<List<GetAllProjectTaskResponse>> GetAllAsync(GetAllProjectTaskQuery query, CancellationToken ct)
    {
        return await db.ProjectTasks.Select(pt => new GetAllProjectTaskResponse(pt.Id, pt.ProjectId, pt.StatusId, pt.Title, pt.Description, pt.Priority.ToString(), pt.Type.ToString(), pt.Order, pt.EstimatePoints, pt.DueDate, pt.CreatedBy, pt.CompanyId)).Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(ct);
    }

    public async Task<GetProjectTaskByIdResponse?> GetByIdAsync(GetProjectTaskByIdQuery query, CancellationToken ct)
    {
        var projectTask = await db.ProjectTasks.Include(pt => pt.Attachments).Include(pt => pt.Comments).Include(pt => pt.Subtasks).Include(pt => pt.Assignees).ThenInclude(a => a.User).FirstOrDefaultAsync(pt => pt.Id == query.Id, ct);

        return projectTask switch
        {
            null => null,
            _ => new GetProjectTaskByIdResponse(projectTask.Id, projectTask.ProjectId, projectTask.StatusId, projectTask.Title, projectTask.Description, projectTask.Priority.ToString(), projectTask.Type.ToString(), projectTask.Order, projectTask.EstimatePoints, projectTask.DueDate, projectTask.CreatedBy, projectTask.CompanyId, projectTask.Attachments.Select(a => new ProjectAttachmentResponse(a.Id, a.FileName, a.ContentType, a.Size)).ToList(),
                projectTask.Comments.Select(c => new ProjectCommentResponse(c.Id, c.Content, c.AuthorId)).ToList(), projectTask.Subtasks.Select(s => new ProjectSubtaskResponse(s.Id, s.Title, s.IsCompleted, s.Order, s.DueDate)).ToList(), projectTask.Assignees.Select(a => new ProjectTaskAssigneeResponse(a.Id, a.UserId, a.User.Name, a.User.Email)).ToList())
        };
    }

    public async Task<AddProjectTaskResponse> AddAsync(AddProjectTaskCommand command, CancellationToken ct)
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
            CreatedBy = command.CreatedBy
        };

        db.ProjectTasks.Add(projectTask);

        await db.SaveChangesAsync(ct);

        return new AddProjectTaskResponse(projectTask.Id, projectTask.ProjectId, projectTask.StatusId, projectTask.Title, projectTask.Description, projectTask.Priority.ToString(), projectTask.Type.ToString(), projectTask.Order, projectTask.EstimatePoints, projectTask.DueDate, projectTask.CreatedBy, projectTask.CompanyId);
    }

    public async Task<UpdateProjectTaskResponse?> UpdateAsync(UpdateProjectTaskCommand command, CancellationToken ct)
    {
        var projectTask = await db.ProjectTasks.FirstOrDefaultAsync(pt => pt.Id == command.Id, ct);

        if (projectTask is null)
        {
            return null;
        }

        projectTask.ProjectId = command.ProjectId;
        projectTask.StatusId = command.StatusId;
        projectTask.Title = command.Title;
        projectTask.Description = command.Description;
        projectTask.Priority = Enum.Parse<EnumTaskPriority>(command.Priority, true);
        projectTask.Type = Enum.Parse<EnumTaskType>(command.Type, true);
        projectTask.Order = command.Order;
        projectTask.EstimatePoints = command.EstimatePoints;
        projectTask.DueDate = command.DueDate;
        projectTask.CreatedBy = command.CreatedBy;

        db.ProjectTasks.Update(projectTask);

        await db.SaveChangesAsync(ct);

        return new UpdateProjectTaskResponse(projectTask.Id, projectTask.ProjectId, projectTask.StatusId, projectTask.Title, projectTask.Description, projectTask.Priority.ToString(), projectTask.Type.ToString(), projectTask.Order, projectTask.EstimatePoints, projectTask.DueDate, projectTask.CreatedBy, projectTask.CompanyId);
    }

    public async Task DeleteAsync(DeleteProjectTaskCommand command, CancellationToken ct)
    {
        var projectTask = await db.ProjectTasks.FirstOrDefaultAsync(pt => pt.Id == command.Id, ct);

        if (projectTask is null)
        {
            return;
        }

        projectTask.Deleted = DateTime.UtcNow;

        db.ProjectTasks.Update(projectTask);

        await db.SaveChangesAsync(ct);
    }
}
