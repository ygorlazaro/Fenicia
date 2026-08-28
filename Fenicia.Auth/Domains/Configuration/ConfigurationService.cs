using Fenicia.Auth.Domains.Configuration.DTOs;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.Configuration;

public class ConfigurationService(ConfigurationRepository repository)
{
    public async Task<List<GetConfigurationResponse>> GetAllAsync(Guid userId, Guid companyId, CancellationToken ct)
    {
        var configurations = await repository.GetByUserAndCompanyAsync(userId, companyId, ct);

        return configurations.Select(c => c.MapToGetConfigurationResponse()).ToList();
    }

    public async Task UpsertAsync(UpsertConfigurationCommand command, CancellationToken ct)
    {
        var configuration = await repository.GetByUserCompanyAndTypeAsync(
            command.UserId, command.CompanyId, command.ConfigType, ct);

        if (configuration is null)
        {
            configuration = new ConfigurationModel
            {
                Id = command.Id ?? Guid.NewGuid(),
                UserId = command.UserId,
                CompanyId = command.CompanyId,
                ConfigType = command.ConfigType,
                Value = command.Value
            };

            await repository.InsertAsync(configuration, ct);
        }
        else
        {
            configuration.Value = command.Value;
            await repository.UpdateAsync(configuration.Id, configuration, ct);
        }
    }
}
