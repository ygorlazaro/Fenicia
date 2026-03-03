using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.Project.Update;

public class UpdateProjectHandler(DefaultContext context)
{
    public async Task<UpdateProjectResponse?> Handle(UpdateProjectCommand command, CancellationToken ct)
    {
        var project = await context.Projects.FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (project is null)
        {
            return null;
        }

        project.Title = command.Title;
        project.Description = command.Description;
        project.Status = Enum.Parse<Common.Enums.Project.ProjectStatus>(command.Status, true);
        project.StartDate = command.StartDate;
        project.EndDate = command.EndDate;
        project.Owner = command.Owner;

        context.Projects.Update(project);

        await context.SaveChangesAsync(ct);

        return new UpdateProjectResponse(
            project.Id,
            project.Title,
            project.Description,
            project.Status.ToString(),
            project.StartDate,
            project.EndDate,
            project.Owner,
            project.CompanyId);
    }
}
