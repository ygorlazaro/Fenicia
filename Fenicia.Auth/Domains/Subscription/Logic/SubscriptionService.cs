using System.Net;

using AutoMapper;

using Fenicia.Auth.Domains.Order.Data;
using Fenicia.Auth.Domains.OrderDetail.Data;
using Fenicia.Auth.Domains.Subscription.Data;
using Fenicia.Auth.Domains.SubscriptionCredit.Data;
using Fenicia.Auth.Enums;
using Fenicia.Common;

namespace Fenicia.Auth.Domains.Subscription.Logic;

/// <summary>
/// Service responsible for managing subscriptions and subscription credits
/// </summary>
public sealed class SubscriptionService(
    IMapper mapper,
    ILogger<SubscriptionService> logger,
    ISubscriptionRepository subscriptionRepository
) : ISubscriptionService
{
    /// <summary>
    /// Creates subscription credits for a given order
    /// </summary>
    /// <param name="order">The order model</param>
    /// <param name="details">List of order details</param>
    /// <param name="companyId">The company identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>API response containing the subscription response</returns>
    public async Task<ApiResponse<SubscriptionResponse>> CreateCreditsForOrderAsync(
        OrderModel order,
        List<OrderDetailModel> details,
        Guid companyId,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Starting credit creation for order {OrderId}", order.Id);

            if (details.Count == 0)
            {
                logger.LogWarning("No modules found for order {OrderId}", order.Id);
                return new ApiResponse<SubscriptionResponse>(
                    null,
                    HttpStatusCode.BadRequest,
                    TextConstants.ThereWasAnErrorAddingModules
                );
            }

            var credits = order
                .Details.Select(d => new SubscriptionCreditModel
                {
                    ModuleId = d.ModuleId,
                    IsActive = true,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(1),
                    OrderDetailId = d.Id
                })
                .ToList();

            var subscription = new SubscriptionModel
            {
                Status = SubscriptionStatus.Active,
                CompanyId = companyId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                OrderId = order.Id,
                Credits = credits
            };

            await subscriptionRepository.SaveSubscriptionAsync(subscription, cancellationToken);
            logger.LogInformation("Successfully saved subscription for order {OrderId}", order.Id);

            var response = mapper.Map<SubscriptionResponse>(subscription);
            return new ApiResponse<SubscriptionResponse>(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating credits for order {OrderId}", order.Id);
            throw;
        }
    }

    /// <summary>
    /// Retrieves all valid subscriptions for a company
    /// </summary>
    /// <param name="companyId">The company identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>API response containing list of valid subscription identifiers</returns>
    public async Task<ApiResponse<List<Guid>>> GetValidSubscriptionsAsync(
        Guid companyId,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Retrieving valid subscriptions for company {CompanyId}", companyId);
            var response = await subscriptionRepository.GetValidSubscriptionAsync(companyId, cancellationToken);
            logger.LogInformation("Found {Count} valid subscriptions for company {CompanyId}", response.Count, companyId);

            return new ApiResponse<List<Guid>>(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving valid subscriptions for company {CompanyId}", companyId);
            throw;
        }
    }
}
