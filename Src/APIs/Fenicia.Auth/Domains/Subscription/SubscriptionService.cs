using Fenicia.Auth.Domains.Subscription.DTOs;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Subscription;

public class SubscriptionService(SubscriptionRepository subscriptionRepository, UserService userService, UserRoleService userRoleService)
{
    public async Task<GetUserProfileResponse?> GetUserProfileAsync(Guid userId, CancellationToken ct)
    {
        var user = await userService.GetByIdAsync(userId, ct);

        if (user is null)
        {
            return null;
        }

        var userRoles = await userRoleService.GetUserRoleModelsByUserAsync(userId, ct);
        var subscriptions = await subscriptionRepository.GetUserSubscriptionsAsync(userId, ct);

        var companies = userRoles.Select(ur => ur.MapToUserCompanyResponse()).ToList();

        var subscriptionResponses = new List<UserSubscriptionResponse>();

        foreach (var subscription in subscriptions)
        {
            var modules = await subscriptionRepository.GetSubscriptionModulesAsync(subscription.Id, ct);
            var moduleResponses = modules.Select(m => m.MapToUserModuleResponse()).ToList();

            subscriptionResponses.Add(subscription.MapToUserSubscriptionResponse(subscription.Company.Name, moduleResponses));
        }

        return new GetUserProfileResponse(user.Id, user.Name, user.Email, companies, subscriptionResponses);
    }

    public async Task<SubscriptionModel> CreateSubscriptionAsync(SubscriptionModel subscription, CancellationToken ct)
    {
        return await subscriptionRepository.InsertAsync(subscription, ct);
    }

    public async Task<List<SubscriptionModel>> GetActiveSubscriptionsForUserAsync(Guid userId, CancellationToken ct)
    {
        return await subscriptionRepository.GetUserSubscriptionsAsync(userId, ct);
    }

    public async Task<List<ModuleModel>> GetActiveModulesForSubscriptionAsync(Guid subscriptionId, CancellationToken ct)
    {
        return await subscriptionRepository.GetSubscriptionModulesAsync(subscriptionId, ct);
    }

    public async Task<List<SubscriptionModel>> GetActiveSubscriptionsByCompanyAsync(Guid companyId, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        return await subscriptionRepository.Query()
            .Where(s => s.CompanyId == companyId && s.Status == SubscriptionStatus.Active && now >= s.StartDate && now <= s.EndDate)
            .ToListAsync(ct);
    }
}
