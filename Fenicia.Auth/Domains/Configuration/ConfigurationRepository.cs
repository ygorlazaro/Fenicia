using Fenicia.Auth.Domains.Configuration.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Enums.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Configuration;

/// <summary>
/// Represents a repository for managing configuration data, providing methods to retrieve configurations based on user, company, and configuration type.
/// </summary>
/// <param name="context"></param>
public class ConfigurationRepository(DbContext context)
    : Repository<ConfigurationModel>(context), IConfigurationRepository
{
    /// <summary>
    /// Returns a configuration based on the specified user, company, and configuration type.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="companyId">The ID of the company.</param>
    /// <param name="configType">The type of the configuration.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The configuration if found, or null if not found.</returns>
    public Task<ConfigurationModel?> GetByUserCompanyAndTypeAsync(Guid userId,
        Guid companyId,
        EnumConfigType configType,
        CancellationToken cancellationToken = default)
    {
        var query = from c in DbSet
                    where c.UserId == userId && c.CompanyId == companyId && c.ConfigType == configType
                    orderby c.ConfigType
                    select c;

        return query.FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Returns a list of configurations based on the specified user and company.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="companyId">The ID of the company.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The list of configurations.</returns>
    public Task<List<ConfigurationModel>> GetByUserAndCompanyAsync(Guid userId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var query = from c in DbSet
                    where c.UserId == userId && c.CompanyId == companyId
                    orderby c.ConfigType
                    select c;

        return query.ToListAsync(cancellationToken);
    }
}
