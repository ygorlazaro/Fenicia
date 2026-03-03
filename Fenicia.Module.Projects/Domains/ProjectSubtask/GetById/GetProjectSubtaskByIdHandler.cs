using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectSubtask.GetById;

public class GetProjectSubtaskByIdHandler(DefaultContext context)
{
    public async Task<GetProjectSubtaskByIdResponse?> Handle(GetProjectSubtaskByIdQuery query, CancellationToken ct)
    {
        var projectSubtask = await context.ProjectSubtasks
            .FirstOrDefaultAsync(ps => ps.Id == query.Id, ct);

        if (projectSubtask is null)
        {
            return null;
        }

        return new GetProjectSubtaskByIdResponse(
            projectSubtask.Id,
            projectSubtask.TaskId,
            projectSubtask.Title,
            projectSubtask.IsCompleted,
            projectSubtask.Order,
            projectSubtask.CompletedAt,
            projectSubtask.CompanyId);
    }
}
