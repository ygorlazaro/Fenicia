using Fenicia.Auth.Domains.Company.Queries;
using Fenicia.Common.Data.Contexts;

using MediatR;

namespace Fenicia.Auth.Domains.Company.Handlers;

/// <summary>
///     Handler responsible for checking if a company exists based on CNPJ.
///     Used during company registration to prevent duplicate CNPJs.
/// </summary>
public class CheckCompanyExistsHandler(DefaultContext db) : IRequestHandler<CheckCompanyExistsQuery, bool>
{
    /// <summary>
    ///     Checks if a company exists with the given CNPJ.
    /// </summary>
    /// <param name="query">The query containing the CNPJ to check and whether to consider only active companies.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if a company exists with the given CNPJ, otherwise false.</returns>
    public virtual async Task<bool> Handle(CheckCompanyExistsQuery query, CancellationToken ct)
    {
        return await db.AuthCompanies.AnyCnpjAsync(query.Cnpj, ct, query.OnlyActive);
    }
}
