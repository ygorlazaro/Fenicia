using Fenicia.Auth.Domains.Subscription.DTOs;
using Fenicia.Auth.Domains.Subscription.Interfaces;
using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Subscription;

public class SubscriptionService(
    ISubscriptionRepository subscriptionRepository,
    IUserService userService,
    IUserRoleService userRoleService) : ISubscriptionService
{
    public async Task<GetUserProfileResponse?> GetUserProfileAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userService.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return null;
        }

        var userRoles = await userRoleService.GetUserRoleModelsByUserAsync(userId, cancellationToken);
        var subscriptions = await subscriptionRepository.GetUserSubscriptionsAsync(userId, cancellationToken);

        var companies = userRoles.Select(ur => ur.MapToUserCompanyResponse()).ToList();

        var subscriptionResponses = new List<UserSubscriptionResponse>();

        foreach (var subscription in subscriptions)
        {
            var modules = await subscriptionRepository.GetSubscriptionModulesAsync(subscription.Id, cancellationToken);
            var moduleResponses = modules.Select(m => m.MapToUserModuleResponse()).ToList();

            subscriptionResponses.Add(
                subscription.MapToUserSubscriptionResponse(subscription.Company.Name, moduleResponses));
        }

        return new GetUserProfileResponse(user.Id, user.Name, user.Email, companies, subscriptionResponses);
    }

    public async Task CreateSubscriptionAsync(
        SubscriptionModel subscription,
        CancellationToken cancellationToken = default)
    {
        await subscriptionRepository.InsertAsync(subscription, cancellationToken);
    }

    public Task<List<ModuleModel>> GetActiveModulesForSubscriptionAsync(
        Guid subscriptionId,
        CancellationToken cancellationToken = default)
    {
        return subscriptionRepository.GetSubscriptionModulesAsync(subscriptionId, cancellationToken);
    }

    public Task<List<SubscriptionModel>> GetActiveSubscriptionsByCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return subscriptionRepository.Query()
            .Where(s => s.CompanyId == companyId && s.Status == SubscriptionStatus.Active && now >= s.StartDate &&
                        now <= s.EndDate)
            .ToListAsync(cancellationToken);
    }
}