using Fenicia.Auth.Domains.Module.Queries;
using Fenicia.Auth.Domains.Module.Responses;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Auth;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Module.Handlers;

/// <summary>
///     Handler responsible for retrieving modules available to a specific user within a company.
///     Returns modules based on the user's active subscriptions and subscription credits.
/// </summary>
/// <remarks>
///     This handler determines which modules a user can access based on:
///     1. The user's role assignments in the company
///     2. Active subscriptions for the company
///     3. Active subscription credits for the modules
///     Only modules with active subscriptions and within the valid date range are returned.
/// </remarks>
public class GetUserModuleHandler(DefaultContext db) : IRequestHandler<GetUserModulesQuery, List<GetUserModulesResponse>>
{
    /// <summary>
    ///     Retrieves modules available to a user for a specific company based on active subscriptions.
    /// </summary>
    /// <param name="query">The query containing company ID and user ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of modules the user has access to through active subscriptions.</returns>
    /// <remarks>
    ///     The query filters for:
    ///     - User has a role in the company
    ///     - Subscription is active
    ///     - Current date is within subscription start and end dates
    ///     - Subscription credit is active
    ///     - Current date is within credit start and end dates
    ///     Results are distinct to avoid duplicate modules.
    /// </remarks>
    public async Task<List<GetUserModulesResponse>> Handle(GetUserModulesQuery query, CancellationToken ct)
    {
        var request = ValidModuleBySubscriptionQuery(query.UserId, query.CompanyId);

        return await request.Distinct().ToListAsync(ct);
    }

    /// <summary>
    ///     Builds a query to find valid modules based on user subscriptions and credits.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="companyId">The company's unique identifier.</param>
    /// <returns>An IQueryable of GetUserModulesResponse containing valid modules.</returns>
    /// <remarks>
    ///     This query joins multiple tables: AuthModules, AuthSubscriptionCredits, AuthSubscriptions, and AuthUserRoles.
    ///     It filters for active subscriptions and credits within their valid date ranges.
    /// </remarks>
    private IQueryable<GetUserModulesResponse> ValidModuleBySubscriptionQuery(Guid userId, Guid companyId)
    {
        var now = DateTime.Now;

        var query = from m in db.AuthModules
                    join sc in db.AuthSubscriptionCredits on m.Id equals sc.ModuleId
                    join s in db.AuthSubscriptions on sc.SubscriptionId equals s.Id
                    join ur in db.AuthUserRoles on s.CompanyId equals ur.CompanyId
                    where ur.UserId == userId &&
                          s.CompanyId == companyId &&
                          s.Status == SubscriptionStatus.Active &&
                          now >= s.StartDate &&
                          now <= s.EndDate &&
                          sc.IsActive &&
                          now >= sc.StartDate &&
                          now <= sc.EndDate
                    select new GetUserModulesResponse(m.Id,
                        m.Name,
                        m.Type);

        return query;
    }
}
