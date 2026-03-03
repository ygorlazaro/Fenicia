using Fenicia.Common.Data.Contexts;

namespace Fenicia.Module.Projects.Domains.Project.Add;

public class AddProjectHandler(DefaultContext context)
{
    public async Task<AddProjectResponse> Handle(AddProjectCommand command, CancellationToken ct)
    {
        var project = new Common.Data.Models.Project
        {
            Id = command.Id,
            Title = command.Title,
            Description = command.Description,
            Status = Enum.Parse<Common.Enums.Project.ProjectStatus>(command.Status, true),
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            Owner = command.Owner
        };

        context.Projects.Add(project);

        await context.SaveChangesAsync(ct);

        return new AddProjectResponse(
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
