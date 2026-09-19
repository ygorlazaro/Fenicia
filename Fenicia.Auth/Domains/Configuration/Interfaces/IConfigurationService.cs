using Fenicia.Common.DTOs.Auth.Configuration;

namespace Fenicia.Auth.Domains.Configuration.Interfaces;

public interface IConfigurationService
{
    Task<List<ConfigurationResponse>> GetAllAsync(
        Guid userId,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task UpsertAsync(ConfigurationRequest request, Guid companyId, CancellationToken cancellationToken = default);
}