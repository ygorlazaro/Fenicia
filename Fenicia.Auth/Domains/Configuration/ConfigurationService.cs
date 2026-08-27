using Fenicia.Auth.Domains.Configuration.Responses;
using Fenicia.Auth.Domains.Configuration.Commands;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Configuration;

public class ConfigurationService(DefaultContext db)
{
    public async Task<List<GetConfigurationResponse>> GetAllAsync(Guid userId, Guid companyId, CancellationToken ct)
    {
        var request = db.AuthConfigurations.Where(c => c.UserId == userId && companyId == c.CompanyId)
            .OrderBy(c => c.ConfigType)
            .Select(c => new GetConfigurationResponse(c.Id,
                c.UserId,
                c.CompanyId,
                c.ConfigType,
                c.Value));

        return await request.ToListAsync(ct);
    }

    public async Task UpsertAsync(UpsertConfigurationCommand command, CancellationToken ct)
    {
        var configuration = await GetCurrentConfigurationAsync(command, ct);

        if (configuration is null)
        {
            AddConfiguration(command);
        }
        else
        {
            UpdateConfiguration(command, configuration);
        }

        await db.SaveChangesAsync(ct);
    }

    private void UpdateConfiguration(UpsertConfigurationCommand command, ConfigurationModel configuration)
    {
        configuration.Value = command.Value;
        db.Entry(configuration).State = EntityState.Modified;
    }

    private void AddConfiguration(UpsertConfigurationCommand command)
    {
        var configuration = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            CompanyId = command.CompanyId,
            ConfigType = command.ConfigType,
            Value = command.Value
        };

        db.AuthConfigurations.Add(configuration);
    }

    private async Task<ConfigurationModel?> GetCurrentConfigurationAsync(UpsertConfigurationCommand command, CancellationToken ct)
    {
        return await db.AuthConfigurations.FirstOrDefaultAsync(
            c => c.UserId == command.UserId && c.ConfigType == command.ConfigType && c.CompanyId == command.CompanyId,
            ct);
    }
}
