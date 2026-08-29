using Fenicia.Auth.Domains.Subscription.DTOs;
using Fenicia.Auth.Domains.User;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.Subscription;

public class SubscriptionService(SubscriptionRepository subscriptionRepository, UserRepository userRepository)
{
    public async Task<GetUserProfileResponse?> GetUserProfileAsync(Guid userId, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(userId, ct);

        if (user is null)
        {
            return null;
        }

        var userRoles = await subscriptionRepository.GetUserRolesAsync(userId, ct);
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
}
