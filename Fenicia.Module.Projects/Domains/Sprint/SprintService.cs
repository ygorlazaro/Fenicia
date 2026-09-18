using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.DTOs.Project.Sprint;
using Fenicia.Module.Projects.Domains.Sprint.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.Sprint;

public class SprintService(ISprintRepository repository, SprintMapper mapper) : ISprintService
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

        return [.. sprints.Select(mapper.MapToGetAllSprintResponse)];
    }

    public async Task<GetSprintByIdResponse?> GetByIdAsync(GetSprintByIdQuery query, CancellationToken cancellationToken = default)
    {
        var sprint = await repository.GetByIdAsync(query.Id, cancellationToken);
        return sprint is null
            ? null
            : mapper.MapToGetSprintByIdResponse(sprint);
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
        return mapper.MapToAddSprintResponse(created);
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
        return updated is not null
            ? mapper.MapToUpdateSprintResponse(updated)
            : null;
    }

    public async Task DeleteAsync(DeleteSprintCommand command, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(command.Id, cancellationToken);
    }
}
