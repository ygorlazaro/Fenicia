using Fenicia.Common.Data.Models.Project;
using Fenicia.Module.Projects.Domains.Sprint.DTOs;
using Fenicia.Module.Projects.Domains.Sprint.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.Sprint;

public class SprintService(ISprintRepository repository) : ISprintService
{
    public async Task<List<GetAllSprintResponse>> GetAllAsync(GetAllSprintQuery query, CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query();
        var filteredQuery = baseQuery;

        if (query.ProjectId.HasValue)
        {
            filteredQuery = filteredQuery.Where(s => s.ProjectId == query.ProjectId.Value);
        }

        var sprints = await filteredQuery
            .OrderByDescending(s => s.StartDate)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);

        return [.. sprints.Select(s => new GetAllSprintResponse(
            s.Id,
            s.ProjectId,
            s.Name,
            s.StartDate,
            s.EndDate,
            s.Description,
            s.CreatedBy,
            s.CompanyId))];
    }

    public async Task<GetSprintByIdResponse?> GetByIdAsync(GetSprintByIdQuery query, CancellationToken cancellationToken = default)
    {
        var sprint = await repository.GetByIdAsync(query.Id, cancellationToken);
        return sprint is null ? null : new GetSprintByIdResponse(
            sprint.Id,
            sprint.ProjectId,
            sprint.Name,
            sprint.StartDate,
            sprint.EndDate,
            sprint.Description,
            sprint.CreatedBy,
            sprint.CompanyId);
    }

    public async Task<AddSprintResponse> AddAsync(AddSprintCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        var sprint = new SprintModel
        {
            Id = command.Id,
            ProjectId = command.ProjectId,
            Name = command.Name,
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            Description = command.Description,
            CreatedBy = command.CreatedBy,
            CompanyId = companyId
        };

        var created = await repository.InsertAsync(sprint, cancellationToken);
        return new AddSprintResponse(
            created.Id,
            created.ProjectId,
            created.Name,
            created.StartDate,
            created.EndDate,
            created.Description,
            created.CreatedBy,
            created.CompanyId);
    }

    public async Task<UpdateSprintResponse?> UpdateAsync(UpdateSprintCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        var sprint = new SprintModel
        {
            Id = command.Id,
            Name = command.Name,
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            Description = command.Description
        };

        var updated = await repository.UpdateAsync(command.Id, sprint, cancellationToken);
        return updated is null ? null : new UpdateSprintResponse(
            updated.Id,
            updated.ProjectId,
            updated.Name,
            updated.StartDate,
            updated.EndDate,
            updated.Description,
            updated.CreatedBy,
            updated.CompanyId);
    }

    public async Task DeleteAsync(DeleteSprintCommand command, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(command.Id, cancellationToken);
    }
}
