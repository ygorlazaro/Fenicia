using Fenicia.Auth.Domains.Subscription.Interfaces;
using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Auth.Domains.Module.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Subscription;
using Fenicia.Common.DTOs.Auth.UserRole;

namespace Fenicia.Auth.Domains.Subscription;

/// <summary>
/// Service implementation for managing subscriptions in the authentication domain.
/// </summary>
/// <param name="repository">The subscription repository.</param>
/// <param name="userService">The user service.</param>
/// <param name="userRoleService">The user role service.</param>
/// <param name="moduleService">The module service.</param>
public class SubscriptionService(
    ISubscriptionRepository repository,
    IUserService userService,
    IUserRoleService userRoleService,
    IModuleService moduleService) : ISubscriptionService
{
    /// <summary>
    /// Gets the user profile with companies and subscriptions.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the user profile.</returns>
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
            var modules = await moduleService.GetActiveModulesForSubscriptionAsync(subscription.Id, cancellationToken);

            var subscriptionResponse = MapToUserSubscriptionResponse(subscription);
            subscriptionResponse.Modules = [.. modules];

            subscriptionResponses.Add(subscriptionResponse);
        }

        return new SubscriptionResponse(user.Id, user.Name, user.Email, companies, subscriptionResponses);
    }

    /// <summary>
    /// Creates a new subscription.
    /// </summary>
    /// <param name="subscription">The subscription model to create.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task CreateSubscriptionAsync(
        SubscriptionModel subscription,
        CancellationToken cancellationToken = default)
    {
        await repository.InsertAsync(subscription, cancellationToken);
    }

    /// <summary>
    /// Gets all active subscriptions for a specific company.
    /// </summary>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of subscriptions.</returns>
    public Task<List<SubscriptionModel>> GetActiveSubscriptionsByCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetActiveSubscriptionsByCompanyAsync(companyId, cancellationToken);
    }

    private static UserCompanyResponse MapToUserCompanyResponse(UserRoleModel userRole)
    {
        return new UserCompanyResponse(
            userRole.Id,
            userRole.Role.Name,
            userRole.CompanyId,
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
}
