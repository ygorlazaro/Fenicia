using Fenicia.Auth.Domains.Configuration.Interfaces;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Configuration;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.Configuration;

public class ConfigurationService(
    IConfigurationRepository repository,
    IUserRoleService userRoleService) : IConfigurationService
{
    public async Task<List<ConfigurationResponse>> GetAllAsync(
        Guid userId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var configurations = await repository.GetByUserAndCompanyAsync(userId, companyId, cancellationToken);

        return [.. configurations.Select(MapToConfigurationResponse)];
    }

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

    private static ConfigurationResponse MapToConfigurationResponse(ConfigurationModel configuration)
    {
        return new ConfigurationResponse(
            configuration.Id,
            configuration.UserId,
            configuration.CompanyId,
            configuration.ConfigType,
            configuration.Value);
    }
}
