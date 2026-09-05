using Fenicia.Module.Projects.Domains.Sprint.DTOs;

namespace Fenicia.Module.Projects.Domains.Sprint.Interfaces;

public interface ISprintService
{
    Task<List<GetAllSprintResponse>> GetAllAsync(GetAllSprintQuery query, CancellationToken cancellationToken = default);

    Task<GetSprintByIdResponse?> GetByIdAsync(GetSprintByIdQuery query, CancellationToken cancellationToken = default);

    Task<AddSprintResponse> AddAsync(AddSprintCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task<UpdateSprintResponse?> UpdateAsync(UpdateSprintCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task DeleteAsync(DeleteSprintCommand command, CancellationToken cancellationToken = default);
}
