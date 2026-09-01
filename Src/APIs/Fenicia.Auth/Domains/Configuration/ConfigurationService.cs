using Fenicia.Auth.Domains.Configuration.DTOs;
using Fenicia.Auth.Domains.Configuration.Interfaces;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.Configuration;

public class ConfigurationService(IConfigurationRepository repository) : IConfigurationService
{
    public async Task<List<GetConfigurationResponse>> GetAllAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default)
    {
        var configurations = await repository.GetByUserAndCompanyAsync(userId, companyId, cancellationToken);

        return [.. configurations.Select(c => c.MapToGetConfigurationResponse())];
    }

    public async Task UpsertAsync(UpsertConfigurationCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        var configuration = await repository.GetByUserCompanyAndTypeAsync(
            command.UserId, companyId, command.ConfigType, cancellationToken);

        if (configuration is null)
        {
            configuration = new ConfigurationModel
            {
                Id = command.Id ?? Guid.NewGuid(),
                UserId = command.UserId,
                CompanyId = companyId,
                ConfigType = command.ConfigType,
                Value = command.Value
            };

            await repository.InsertAsync(configuration, cancellationToken);
        }
        else
        {
            configuration.Value = command.Value;
            await repository.UpdateAsync(configuration.Id, configuration, cancellationToken);
        }
    }
}
