using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Configuration.Interfaces;

/// <summary>
/// Defines the contract for a repository that manages configuration data, allowing retrieval of configurations based on user, company, and configuration type.
/// </summary>
public interface IConfigurationRepository : IRepository<ConfigurationModel>
{
    /// <summary>
    /// Retrieves a configuration based on the specified user, company, and configuration type.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="companyId">The ID of the company.</param>
    /// <param name="configType">The type of the configuration.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The configuration if found, or null if not found.</returns>
    Task<ConfigurationModel?> GetByUserCompanyAndTypeAsync(Guid userId,
        Guid companyId,
        EnumConfigType configType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of configurations based on the specified user and company.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="companyId">The ID of the company.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The list of configurations.</returns>
    Task<List<ConfigurationModel>> GetByUserAndCompanyAsync(Guid userId,
        Guid companyId,
        CancellationToken cancellationToken = default);
}
