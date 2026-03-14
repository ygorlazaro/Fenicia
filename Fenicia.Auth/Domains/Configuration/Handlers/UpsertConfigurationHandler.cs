using Fenicia.Auth.Domains.Configuration.Commands;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Configuration.Handlers;

/// <summary>
///     Handler responsible for creating or updating configuration entries.
///     Uses upsert pattern: creates new configuration if combination of UserId, CompanyId, and ConfigType doesn't exist,
///     otherwise updates the existing configuration value.
/// </summary>
public class UpsertConfigurationHandler(DefaultContext db)
{
    /// <summary>
    ///     Handles the upsert configuration operation.
    ///     Creates a new configuration if it doesn't exist, or updates the existing one.
    /// </summary>
    /// <param name="command">The upsert command containing configuration details.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Handle(UpsertConfigurationCommand command, CancellationToken ct)
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

    /// <summary>
    ///     Updates an existing configuration's value.
    /// </summary>
    /// <param name="command">The upsert command with new value.</param>
    /// <param name="configuration">The existing configuration to update.</param>
    private void UpdateConfiguration(UpsertConfigurationCommand command, ConfigurationModel configuration)
    {
        configuration.Value = command.Value;
        db.Entry(configuration).State = EntityState.Modified;
    }

    /// <summary>
    ///     Creates a new configuration entry.
    /// </summary>
    /// <param name="command">The upsert command with configuration details.</param>
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

    /// <summary>
    ///     Retrieves an existing configuration matching the UserId, ConfigType, and CompanyId.
    /// </summary>
    /// <param name="command">The command to match against.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The existing configuration or null if not found.</returns>
    private async Task<ConfigurationModel?> GetCurrentConfigurationAsync(UpsertConfigurationCommand command, CancellationToken ct)
    {
        return await db.AuthConfigurations.FirstOrDefaultAsync(c => c.UserId == command.UserId && c.ConfigType == command.ConfigType && c.CompanyId == command.CompanyId, ct);
    }
}