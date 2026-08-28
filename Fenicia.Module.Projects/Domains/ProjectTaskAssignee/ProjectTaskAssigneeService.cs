using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Enums.Project;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.DTOs;

namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee;

public class ProjectTaskAssigneeService(ProjectTaskAssigneeRepository repository)
{
    public async Task<List<GetAllProjectTaskAssigneeResponse>> GetAllAsync(GetAllProjectTaskAssigneeQuery query, CancellationToken ct)
    {
        var assignees = await repository.GetAllAsync(query.Page, query.PerPage, ct);
        return assignees.Select(a => new GetAllProjectTaskAssigneeResponse(a.Id, a.TaskId, a.UserId, a.Role.ToString(), a.AssignedAt, a.CompanyId)).ToList();
    }

    public async Task<GetProjectTaskAssigneeByIdResponse?> GetByIdAsync(GetProjectTaskAssigneeByIdQuery query, CancellationToken ct)
    {
        var assignee = await repository.GetByIdAsync(query.Id, ct);

        return assignee switch
        {
            null => null,
            _ => new GetProjectTaskAssigneeByIdResponse(assignee.Id, assignee.TaskId, assignee.UserId, assignee.Role.ToString(), assignee.AssignedAt, assignee.CompanyId)
        };
    }

    public async Task<AddProjectTaskAssigneeResponse> AddAsync(AddProjectTaskAssigneeCommand command, Guid companyId, CancellationToken ct)
    {
        var assignee = new TaskAssigneeModel
        {
            Id = command.Id,
            TaskId = command.TaskId,
            UserId = command.UserId,
            Role = Enum.Parse<EnumAssigneeRole>(command.Role, true),
            AssignedAt = command.AssignedAt,
            CompanyId = companyId
        };

        var created = await repository.InsertAsync(assignee, ct);
        return new AddProjectTaskAssigneeResponse(created.Id, created.TaskId, created.UserId, created.Role.ToString(), created.AssignedAt, created.CompanyId);
    }

    public async Task<UpdateProjectTaskAssigneeResponse?> UpdateAsync(UpdateProjectTaskAssigneeCommand command, Guid companyId, CancellationToken ct)
    {
        var assignee = new TaskAssigneeModel
        {
            Id = command.Id,
            TaskId = command.TaskId,
            UserId = command.UserId,
            Role = Enum.Parse<EnumAssigneeRole>(command.Role, true),
            AssignedAt = command.AssignedAt,
            CompanyId = companyId
        };

        var updated = await repository.UpdateAsync(command.Id, assignee, ct);
        return updated is null ? null : new UpdateProjectTaskAssigneeResponse(updated.Id, updated.TaskId, updated.UserId, updated.Role.ToString(), updated.AssignedAt, updated.CompanyId);
    }

    public async Task DeleteAsync(DeleteProjectTaskAssigneeCommand command, CancellationToken ct)
    {
        await repository.DeleteAsync(command.Id, ct);
    }
}
