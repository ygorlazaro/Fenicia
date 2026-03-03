using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectTask.GetById;

public class GetProjectTaskByIdHandler(DefaultContext context)
{
    public async Task<GetProjectTaskByIdResponse?> Handle(GetProjectTaskByIdQuery query, CancellationToken ct)
    {
        var projectTask = await context.ProjectTasks
            .Include(pt => pt.Attachments)
            .Include(pt => pt.Comments)
            .Include(pt => pt.Subtasks)
            .Include(pt => pt.Assignees)
                .ThenInclude(a => a.User)
            .FirstOrDefaultAsync(pt => pt.Id == query.Id, ct);

        if (projectTask is null)
        {
            return null;
        }

        return new GetProjectTaskByIdResponse(
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
            projectTask.Attachments.Select(a => new ProjectAttachmentResponse(
                a.Id,
                a.FileName,
                a.ContentType,
                a.Size)).ToList(),
            projectTask.Comments.Select(c => new ProjectCommentResponse(
                c.Id,
                c.Content,
                c.AuthorId)).ToList(),
            projectTask.Subtasks.Select(s => new ProjectSubtaskResponse(
                s.Id,
                s.Title,
                s.IsCompleted,
                s.Order,
                s.DueDate)).ToList(),
            projectTask.Assignees.Select(a => new ProjectTaskAssigneeResponse(
                a.Id,
                a.UserId,
                a.User.Name,
                a.User.Email)).ToList());
    }
}
