using Fenicia.Auth.Domains.Configuration.Interfaces;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Configuration;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.Configuration;

/// <summary>
/// Service class for managing configuration settings for users and companies.
/// </summary>
/// <param name="repository">The configuration repository.</param>
/// <param name="userRoleService">The user role service.</param>
public class ConfigurationService(
    IConfigurationRepository repository,
    IUserRoleService userRoleService) : IConfigurationService
{
    /// <summary>
    /// Retrieves all configuration settings for a specific user and company.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="companyId">The company ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The list of configuration responses.</returns>
    public async Task<IEnumerable<ConfigurationResponse>> GetAllAsync(
        Guid userId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var configurations = await repository.GetByUserAndCompanyAsync(userId, companyId, cancellationToken);

        return [.. configurations.Select(ConfigurationMapper.MapToConfigurationResponse)];
    }

    /// <summary>
    /// Upserts a configuration setting for a specific user and company. If the configuration exists, it updates the value; otherwise, it creates a new configuration.
    /// </summary>
    /// <param name="request">The configuration request.</param>
    /// <param name="companyId">The company ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="PermissionDeniedException"></exception>
    public async Task UpsertAsync(
        ConfigurationRequest request,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var isAdmin = await userRoleService.IsAdminAsync(request.UserId, companyId, cancellationToken);

        if (!isAdmin)
        {
            throw new PermissionDeniedException(ExceptionMessages.PermissionDeniedUpdateCompany);
        }

        var configuration = await repository.GetByUserCompanyAndTypeAsync(
            request.UserId,
            companyId,
            request.ConfigType,
            cancellationToken);

        if (configuration is null)
        {
            configuration = new ConfigurationModel
            {
                UserId = request.UserId,
                CompanyId = companyId,
                ConfigType = request.ConfigType,
                Value = request.Value
            };

            await repository.InsertAsync(configuration, cancellationToken);

            return;
        }

        configuration.Value = request.Value;
        await repository.UpdateAsync(configuration.Id, configuration, cancellationToken);
    }
}
