using Fenicia.Auth.Domains.Configuration.DTOs;

namespace Fenicia.Auth.Domains.Configuration.Interfaces;

public interface IConfigurationService
{
    Task<List<GetConfigurationResponse>> GetAllAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default);

    Task UpsertAsync(UpsertConfigurationCommand command, Guid companyId, CancellationToken cancellationToken = default);
}
