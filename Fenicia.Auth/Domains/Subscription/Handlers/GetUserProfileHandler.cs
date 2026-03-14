using Fenicia.Auth.Domains.Subscription.Queries;
using Fenicia.Auth.Domains.Subscription.Responses;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Subscription.Handlers;

public class GetUserProfileHandler(DefaultContext db)
{
    public async Task<GetUserProfileResponse?> Handle(GetUserProfileQuery query, CancellationToken ct)
    {
        var user = await db.AuthUsers.FirstOrDefaultAsync(u => u.Id == query.UserId, ct);

        if (user is null)
        {
            return null;
        }

        var userCompanies = await GetUserCompaniesAsync(query.UserId, ct);
        var subscriptions = await GetUserSubscriptionsAsync(query, ct);

        return new GetUserProfileResponse(user.Id, user.Name, user.Email, userCompanies, subscriptions);
    }

    private async Task<List<UserSubscriptionResponse>> GetUserSubscriptionsAsync(GetUserProfileQuery query, CancellationToken ct)
    {
        var request = from s in db.AuthSubscriptions join c in db.AuthCompanies on s.CompanyId equals c.Id join ur in db.AuthUserRoles on c.Id equals ur.CompanyId where ur.UserId == query.UserId select new UserSubscriptionResponse(s.Id, c.Id, c.Name, s.Status, s.StartDate, s.EndDate);

        return await request.ToListAsync(ct);
    }

    private async Task<List<UserCompanyResponse>> GetUserCompaniesAsync(Guid userId, CancellationToken ct)
    {
        var request = from ur in db.AuthUserRoles join c in db.AuthCompanies on ur.CompanyId equals c.Id where ur.UserId == userId select new UserCompanyResponse(c.Id, c.Name, c.Cnpj);

        var companies = await request.ToListAsync(ct);

        return companies;
    }
}