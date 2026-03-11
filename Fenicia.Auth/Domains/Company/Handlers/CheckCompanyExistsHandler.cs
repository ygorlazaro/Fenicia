using Fenicia.Common.Data.Contexts;

namespace Fenicia.Auth.Domains.Company.CheckCompanyExists;

public class CheckCompanyExistsHandler(DefaultContext db)
{
    public virtual async Task<bool> Handle(CheckCompanyExistsQuery query, CancellationToken ct)
    {
        return await db.AuthCompanies.AnyCnpjAsync(query.Cnpj, ct, query.OnlyActive);
    }
}
