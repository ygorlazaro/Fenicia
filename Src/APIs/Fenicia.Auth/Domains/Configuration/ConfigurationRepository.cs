using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Enums.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Configuration;

public class ConfigurationRepository(DefaultContext context) : Repository<ConfigurationModel>(context)
{
    public async Task<ConfigurationModel?> GetByUserCompanyAndTypeAsync(Guid userId, Guid companyId, ConfigType configType, CancellationToken ct = default)
    {
        return await DbSet.FirstOrDefaultAsync(
            c => c.UserId == userId && c.CompanyId == companyId && c.ConfigType == configType,
            ct);
    }

    public async Task<List<ConfigurationModel>> GetByUserAndCompanyAsync(Guid userId, Guid companyId, CancellationToken ct = default)
    {
        var query = from c in DbSet
                    where c.UserId == userId && c.CompanyId == companyId
                    orderby c.ConfigType
                    select c;

        return await query.ToListAsync(ct);
    }
}
