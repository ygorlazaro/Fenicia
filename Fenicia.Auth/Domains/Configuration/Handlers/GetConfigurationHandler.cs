using Fenicia.Auth.Domains.Configuration.Queries;
using Fenicia.Auth.Domains.Configuration.Responses;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Configuration.Handlers;

public class GetConfigurationHandler(DefaultContext db)
{
    public async Task<List<GetConfigurationResponse>> Handle(GetConfigurationQuery query, CancellationToken ct)
    {
        var request = from c in db.AuthConfigurations
                      where c.UserId == query.UserId
                            && (query.CompanyId == null || c.CompanyId == query.CompanyId)
                      orderby c.ConfigType
                      select new GetConfigurationResponse(c.Id,
                          c.UserId,
                          c.CompanyId,
                          c.ConfigType,
                          c.Value);

        return await request.ToListAsync(ct);
    }
}
