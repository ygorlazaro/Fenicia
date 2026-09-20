using Fenicia.Auth.Domains.Subscription.Interfaces;
using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Module;
using Fenicia.Common.DTOs.Auth.Subscription;

namespace Fenicia.Auth.Domains.Subscription;

public class SubscriptionService(
    ISubscriptionRepository repository,
    IUserService userService,
    IUserRoleService userRoleService) : ISubscriptionService
{
    public async Task<SubscriptionResponse?> GetUserProfileAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userService.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return null;
        }

        var userRoles = await userRoleService.GetUserRoleModelsByUserAsync(userId, cancellationToken);
        var subscriptions = await repository.GetUserSubscriptionsAsync(userId, cancellationToken);

        var companies = userRoles.Select(MapToUserCompanyResponse).ToList();

        var subscriptionResponses = new List<UserSubscriptionResponse>();

        foreach (var subscription in subscriptions)
        {
            var modules = await repository.GetSubscriptionModulesAsync(subscription.Id, cancellationToken);
            var moduleResponses = modules.Select(MapToModuleResponse).ToList();

            var subscriptionResponse = MapToUserSubscriptionResponse(subscription);
            subscriptionResponse.Modules = moduleResponses;

            subscriptionResponses.Add(subscriptionResponse);
        }

        return new SubscriptionResponse(user.Id, user.Name, user.Email, companies, subscriptionResponses);
    }

    public async Task CreateSubscriptionAsync(
        SubscriptionModel subscription,
        CancellationToken cancellationToken = default)
    {
        await repository.InsertAsync(subscription, cancellationToken);
    }

    public Task<List<ModuleModel>> GetActiveModulesForSubscriptionAsync(
        Guid subscriptionId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetSubscriptionModulesAsync(subscriptionId, cancellationToken);
    }

    public Task<List<SubscriptionModel>> GetActiveSubscriptionsByCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetActiveSubscriptionsByCompanyAsync(companyId, cancellationToken);
    }

    private static UserCompanyResponse MapToUserCompanyResponse(UserRoleModel userRole)
    {
        return new UserCompanyResponse(
            userRole.Company.Id,
            userRole.Company.Name,
            userRole.Company.Cnpj);
    }

    private static UserSubscriptionResponse MapToUserSubscriptionResponse(SubscriptionModel subscription)
    {
        return new UserSubscriptionResponse(
            subscription.Id,
            subscription.CompanyId,
            subscription.Company.Name,
            subscription.Status,
            subscription.StartDate,
            subscription.EndDate);
    }

    private static ModuleResponse MapToModuleResponse(ModuleModel module)
    {
        return new ModuleResponse(
            module.Id,
            module.Name,
            module.Type,
            module.Description,
            module.IsActive,
            module.SortOrder,
            module.Price);
    }
}
