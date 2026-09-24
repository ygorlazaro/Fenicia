using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.DTOs.Project.Project;
using Fenicia.Common.Enums.Project;
using Fenicia.Module.Projects.Domains.Project.Interfaces;

namespace Fenicia.Module.Projects.Domains.Project;

public class ProjectService(IProjectRepository repository) : IProjectService
{
    public async Task<List<ProjectResponse>> GetAllAsync(
        GetAllProjectQuery query,
        CancellationToken cancellationToken = default)
    {
        var projects = await repository.GetAllAsync(query.Page, query.PerPage, cancellationToken);
        return [.. projects.Select(p => new ProjectResponse(
            p.Id,
            p.Title,
            p.Description,
            p.Status.ToString(),
            p.StartDate,
            p.EndDate,
            p.Owner,
            p.CompanyId))];
    }

    public async Task<ProjectResponse?> GetByIdAsync(
        GetProjectByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var project = await repository.GetByIdWithRelationsAsync(query.Id, cancellationToken);

        return project is null
            ? null
            : new ProjectResponse(
                project.Id,
                project.Title,
                project.Description,
                project.Status.ToString(),
                project.StartDate,
                project.EndDate,
                project.Owner,
                project.CompanyId,
                [.. project.Statuses.Select(s => new ProjectStatusResponse(s.Id, s.Name, s.Color, s.Order, s.IsFinal))],
                [.. project.Tasks.Select(t => new ProjectTaskResponse(
                    t.Id,
                    t.Title,
                    t.Description,
                    t.Priority.ToString(),
                    t.Type.ToString(),
                    t.EstimatePoints,
                    t.DueDate))]);
    }

    public async Task<ProjectResponse> AddAsync(
        ProjectRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var project = new ProjectModel
        {
            Id = command.Id ?? Guid.NewGuid(),
            Title = command.Title,
            Description = command.Description,
            Status = Enum.Parse<EnumProjectStatus>(command.Status, true),
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            Owner = command.Owner,
            CompanyId = companyId
        };

        var created = await repository.InsertAsync(project, cancellationToken);
        return new ProjectResponse(
            created.Id,
            created.Title,
            created.Description,
            created.Status.ToString(),
            created.StartDate,
            created.EndDate,
            created.Owner,
            created.CompanyId);
    }

    public async Task<ProjectResponse?> UpdateAsync(
        ProjectRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var project = new ProjectModel
        {
            Id = command.Id!.Value,
            Title = command.Title,
            Description = command.Description,
            Status = Enum.Parse<EnumProjectStatus>(command.Status, true),
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            Owner = command.Owner,
            CompanyId = companyId
        };

        var updated = await repository.UpdateAsync(command.Id.Value, project, cancellationToken);
        return updated is null
            ? null
            : new ProjectResponse(
                updated.Id,
                updated.Title,
                updated.Description,
                updated.Status.ToString(),
                updated.StartDate,
                updated.EndDate,
                updated.Owner,
                updated.CompanyId);
    }

    public async Task DeleteAsync(DeleteProjectCommand command, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(command.Id, cancellationToken);
    }
}
