using Fenicia.Auth.Domains.Configuration.Queries;
using Fenicia.Auth.Domains.Configuration.Responses;
using Fenicia.Common.Data.Contexts;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Configuration.Handlers;

/// <summary>
///     Handler responsible for retrieving user configurations.
///     Returns configurations filtered by user and optionally by company, ordered by ConfigType.
/// </summary>
public class GetConfigurationHandler(DefaultContext db) : IRequestHandler<GetConfigurationQuery, List<GetConfigurationResponse>>
{
    /// <summary>
    ///     Retrieves configurations for a user, filtered by company.
    /// </summary>
    /// <param name="query">The query containing user ID and required company ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of configuration responses ordered by ConfigType.</returns>
    public async Task<List<GetConfigurationResponse>> Handle(GetConfigurationQuery query, CancellationToken ct)
    {
        var request = db.AuthConfigurations.Where(c => c.UserId == query.UserId && query.CompanyId == c.CompanyId)
            .OrderBy(c => c.ConfigType)
            .Select(c => new GetConfigurationResponse(c.Id,
                c.UserId,
                c.CompanyId,
                c.ConfigType,
                c.Value));

        return await request.ToListAsync(ct);
    }
}
