namespace Fenicia.Auth.Domains.Subscription.Logic;

using System.Net;

using Common;

using Data;

using Enums;

using Order.Data;

using OrderDetail.Data;

using SubscriptionCredit.Data;

public sealed class SubscriptionService(ILogger<SubscriptionService> logger, ISubscriptionRepository subscriptionRepository) : ISubscriptionService
{
    public async Task<ApiResponse<SubscriptionResponse>> CreateCreditsForOrderAsync(OrderModel order, List<OrderDetailModel> details, Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Starting credit creation for order {OrderId}", order.Id);

            if (details.Count == 0)
            {
                logger.LogWarning(message: "No modules found for order {OrderId}", order.Id);
                return new ApiResponse<SubscriptionResponse>(data: null, HttpStatusCode.BadRequest, TextConstants.ThereWasAnErrorAddingModules);
            }

            var credits = order.Details.Select(d => new SubscriptionCreditModel
            {
                ModuleId = d.ModuleId,
                IsActive = true,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(months: 1),
                OrderDetailId = d.Id
            }).ToList();

            var subscription = new SubscriptionModel
            {
                Status = SubscriptionStatus.Active,
                CompanyId = companyId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(months: 1),
                OrderId = order.Id,
                Credits = credits
            };

            await subscriptionRepository.SaveSubscriptionAsync(subscription, cancellationToken);
            logger.LogInformation(message: "Successfully saved subscription for order {OrderId}", order.Id);

            var response = SubscriptionResponse.Convert(subscription);
            return new ApiResponse<SubscriptionResponse>(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error creating credits for order {OrderId}", order.Id);
            throw;
        }
    }

    public async Task<ApiResponse<List<Guid>>> GetValidSubscriptionsAsync(Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Retrieving valid subscriptions for company {CompanyId}", companyId);
            var response = await subscriptionRepository.GetValidSubscriptionAsync(companyId, cancellationToken);
            logger.LogInformation(message: "Found {Count} valid subscriptions for company {CompanyId}", response.Count, companyId);

            return new ApiResponse<List<Guid>>(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error retrieving valid subscriptions for company {CompanyId}", companyId);
            throw;
        }
    }
}
