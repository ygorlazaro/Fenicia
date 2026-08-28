using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.DTOs;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Enums.Project;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee;

public class ProjectTaskAssigneeService(DefaultContext db)
{
    public async Task<List<GetAllProjectTaskAssigneeResponse>> GetAllAsync(GetAllProjectTaskAssigneeQuery query, CancellationToken ct)
    {
        return await db.ProjectTaskAssignees.Select(a => new GetAllProjectTaskAssigneeResponse(a.Id, a.TaskId, a.UserId, a.Role.ToString(), a.AssignedAt, a.CompanyId)).Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(ct);
    }

    public async Task<GetProjectTaskAssigneeByIdResponse?> GetByIdAsync(GetProjectTaskAssigneeByIdQuery query, CancellationToken ct)
    {
        var assignee = await db.ProjectTaskAssignees.FirstOrDefaultAsync(a => a.Id == query.Id, ct);

        return assignee switch
        {
            null => null,
            _ => new GetProjectTaskAssigneeByIdResponse(assignee.Id, assignee.TaskId, assignee.UserId, assignee.Role.ToString(), assignee.AssignedAt, assignee.CompanyId)
        };
    }

    public async Task<AddProjectTaskAssigneeResponse> AddAsync(AddProjectTaskAssigneeCommand command, CancellationToken ct)
    {
        var assignee = new TaskAssigneeModel
        {
            Id = command.Id,
            TaskId = command.TaskId,
            UserId = command.UserId,
            Role = Enum.Parse<EnumAssigneeRole>(command.Role, true),
            AssignedAt = command.AssignedAt
        };

        db.ProjectTaskAssignees.Add(assignee);

        await db.SaveChangesAsync(ct);

        return new AddProjectTaskAssigneeResponse(assignee.Id, assignee.TaskId, assignee.UserId, assignee.Role.ToString(), assignee.AssignedAt, assignee.CompanyId);
    }

    public async Task<UpdateProjectTaskAssigneeResponse?> UpdateAsync(UpdateProjectTaskAssigneeCommand command, CancellationToken ct)
    {
        var assignee = await db.ProjectTaskAssignees.FirstOrDefaultAsync(a => a.Id == command.Id, ct);

        if (assignee is null)
        {
            return null;
        }

        assignee.TaskId = command.TaskId;
        assignee.UserId = command.UserId;
        assignee.Role = Enum.Parse<EnumAssigneeRole>(command.Role, true);
        assignee.AssignedAt = command.AssignedAt;

        db.ProjectTaskAssignees.Update(assignee);

        await db.SaveChangesAsync(ct);

        return new UpdateProjectTaskAssigneeResponse(assignee.Id, assignee.TaskId, assignee.UserId, assignee.Role.ToString(), assignee.AssignedAt, assignee.CompanyId);
    }

    public async Task DeleteAsync(DeleteProjectTaskAssigneeCommand command, CancellationToken ct)
    {
        var assignee = await db.ProjectTaskAssignees.FirstOrDefaultAsync(a => a.Id == command.Id, ct);

        if (assignee is null)
        {
            return;
        }

        assignee.Deleted = DateTime.UtcNow;

        db.ProjectTaskAssignees.Update(assignee);

        await db.SaveChangesAsync(ct);
    }
}
