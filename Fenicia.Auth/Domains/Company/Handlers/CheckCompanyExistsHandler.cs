using Fenicia.Auth.Domains.Company.Queries;
using Fenicia.Common.Data.Contexts;

using MediatR;

namespace Fenicia.Auth.Domains.Company.Handlers;

public class CheckCompanyExistsHandler(DefaultContext db) : IRequestHandler<CheckCompanyExistsQuery, bool>
{

    public virtual async Task<bool> Handle(CheckCompanyExistsQuery query, CancellationToken ct)
    {
        return await db.AuthCompanies.AnyCnpjAsync(query.Cnpj, ct, query.OnlyActive);
    }
}
