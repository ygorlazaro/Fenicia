using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Configuration.GetConfiguration;

public class GetConfigurationHandler(DefaultContext context)
{
    public async Task<List<GetConfigurationResponse>> Handle(GetConfigurationQuery query, CancellationToken ct)
    {
        var configurations = await context.AuthConfiguration
            .Where(c => c.UserId == query.UserId && 
                       (query.CompanyId == null || c.CompanyId == query.CompanyId))
            .OrderBy(c => c.ConfigType)
            .Select(c => new GetConfigurationResponse(
                c.Id,
                c.UserId,
                c.CompanyId,
                c.ConfigType,
                c.Value
            ))
            .ToListAsync(ct);

        return configurations;
    }
}
