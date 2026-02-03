using System.Net;

using Fenicia.Common;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Responses;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.Subscription;

public sealed class SubscriptionService(ISubscriptionRepository subscriptionRepository) : ISubscriptionService
{
    public async Task<ApiResponse<SubscriptionResponse>> CreateCreditsForOrderAsync(OrderModel order, List<OrderDetailModel> details, Guid companyId, CancellationToken cancellationToken)
    {
        if (details.Count == 0)
        {
            return new ApiResponse<SubscriptionResponse>(data: null, HttpStatusCode.BadRequest, TextConstants.ThereWasAnErrorAddingModulesMessage);
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

        var response = SubscriptionResponse.Convert(subscription);

        return new ApiResponse<SubscriptionResponse>(response);
    }

    public async Task<ApiResponse<List<Guid>>> GetValidSubscriptionsAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var response = await subscriptionRepository.GetValidSubscriptionAsync(companyId, cancellationToken);

        return new ApiResponse<List<Guid>>(response);
    }
}
