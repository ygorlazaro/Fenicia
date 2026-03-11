using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Configuration.UpsertConfiguration;

public class UpsertConfigurationHandler(DefaultContext context)
{
    public async Task Handle(UpsertConfigurationCommand command, CancellationToken ct)
    {
        if (command.CompanyId is null)
        {
            throw new InvalidRequestException(ExceptionMessages.CompanyNotFoundMessage);
        }
        
        var configuration = await context.AuthConfiguration
            .FirstOrDefaultAsync(c => 
                c.UserId == command.UserId && 
                c.ConfigType == command.ConfigType &&
                c.CompanyId == command.CompanyId, 
                ct);

        if (configuration is null)
        {
            // Insert - Create new configuration
            configuration = new ConfigurationModel
            {
                Id = Guid.NewGuid(),
                UserId = command.UserId,
                CompanyId = command.CompanyId.Value,
                ConfigType = command.ConfigType,
                Value = command.Value
            };

            context.AuthConfiguration.Add(configuration);
        }
        else
        {
            // Update - Modify existing configuration
            configuration.Value = command.Value;
            context.Entry(configuration).State = EntityState.Modified;
        }

        await context.SaveChangesAsync(ct);
    }
}
