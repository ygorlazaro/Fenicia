using Fenicia.Common.DTOs.Auth.Configuration;

namespace Fenicia.Auth.Domains.Configuration.Interfaces;

/// <summary>
/// Defines the contract for a service that manages configuration data, allowing retrieval and upsertion of configurations based on user and company.
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// Retrieves all configurations for a specific user and company.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="companyId">The ID of the company.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The list of configurations.</returns>
    Task<IEnumerable<ConfigurationResponse>> GetAllAsync(
        Guid userId,
        Guid companyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates or updates a configuration (upsert) for the authenticated user.
    /// </summary>
    /// <param name="request">The request containing the configuration data.</param>
    /// <param name="companyId">The ID of the company.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpsertAsync(ConfigurationRequest request, Guid companyId, CancellationToken cancellationToken = default);
}
