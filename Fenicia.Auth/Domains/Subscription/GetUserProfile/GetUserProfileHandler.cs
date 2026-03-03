using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Subscription.GetUserProfile;

public class GetUserProfileHandler(DefaultContext context)
{
    public async Task<GetUserProfileResponse?> Handle(GetUserProfileQuery query, CancellationToken ct)
    {
        var user = await context.AuthUsers
            .FirstOrDefaultAsync(u => u.Id == query.UserId, ct);

        if (user is null)
        {
            return null;
        }

        var userCompanies = await context.Companies
            .Where(c => c.UsersRoles.Any(u => u.Id == query.UserId))
            .Select(c => new UserCompanyResponse
            {
                Id = c.Id,
                Name = c.Name,
                Cnpj = c.Cnpj
            })
            .ToListAsync(ct);

        var subscriptions = await context.Subscriptions
            .Include(s => s.Credits)
                .ThenInclude(c => c.ModuleModel)
            .Include(s => s.CompanyModel)
            .Where(s => s.CompanyModel.UsersRoles.Any(u => u.Id == query.UserId))
            .Select(s => new UserSubscriptionResponse
            {
                Id = s.Id,
                CompanyId = s.CompanyId,
                CompanyName = s.CompanyModel.Name,
                Status = s.Status.ToString(),
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                Modules = s.Credits
                    .Where(c => c.IsActive)
                    .Select(c => new SubscribedModuleResponse
                    {
                        Id = c.ModuleModel.Id,
                        Name = c.ModuleModel.Name,
                        Type = c.ModuleModel.Type.ToString(),
                        SubscribedAt = c.StartDate
                    })
                    .ToList()
            })
            .ToListAsync(ct);

        return new GetUserProfileResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Companies = userCompanies,
            Subscriptions = subscriptions
        };
    }
}
