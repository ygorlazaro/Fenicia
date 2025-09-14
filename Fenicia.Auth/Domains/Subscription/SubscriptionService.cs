namespace Fenicia.Auth.Domains.Subscription;

using System.Net;

using Common;
using Common.Database.Responses;

using Fenicia.Common.Database.Models.Auth;
using Common.Enums;

public sealed class SubscriptionService : ISubscriptionService
{
    private readonly ILogger<SubscriptionService> _logger;
    private readonly ISubscriptionRepository _subscriptionRepository;

    public SubscriptionService(ILogger<SubscriptionService> logger, ISubscriptionRepository subscriptionRepository)
    {
        _logger = logger;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<ApiResponse<SubscriptionResponse>> CreateCreditsForOrderAsync(OrderModel order, List<OrderDetailModel> details, Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting credit creation for order {OrderID}", order.Id);

            if (details.Count == 0)
            {
                _logger.LogWarning("No modules found for order {OrderID}", order.Id);
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

            await _subscriptionRepository.SaveSubscriptionAsync(subscription, cancellationToken);
            _logger.LogInformation("Successfully saved subscription for order {OrderID}", order.Id);

            var response = SubscriptionResponse.Convert(subscription);
            return new ApiResponse<SubscriptionResponse>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating credits for order {OrderID}", order.Id);
            throw;
        }
    }

    public async Task<ApiResponse<List<Guid>>> GetValidSubscriptionsAsync(Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving valid subscriptions for company {CompanyID}", companyId);
            var response = await _subscriptionRepository.GetValidSubscriptionAsync(companyId, cancellationToken);
            _logger.LogInformation("Found {Count} valid subscriptions for company {CompanyID}", response.Count, companyId);

            return new ApiResponse<List<Guid>>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving valid subscriptions for company {CompanyID}", companyId);
            throw;
        }
    }
}
