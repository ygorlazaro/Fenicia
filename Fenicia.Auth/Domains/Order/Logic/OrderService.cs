namespace Fenicia.Auth.Domains.Order.Logic;

using System.Net;

using AutoMapper;

using Common;
using Common.Enums;

using Data;

using Enums;

using Module.Data;
using Module.Logic;

using OrderDetail.Data;

using Subscription.Logic;

using User.Logic;

/// <summary>
///     Service responsible for managing orders and their creation process
/// </summary>
public sealed class OrderService(IMapper mapper, ILogger<OrderService> logger, IOrderRepository orderRepository, IModuleService moduleService, ISubscriptionService subscriptionService, IUserService userService) : IOrderService
{
    /// <summary>
    ///     Creates a new order for the specified user and company
    /// </summary>
    /// <param name="userId">The ID of the user creating the order</param>
    /// <param name="companyId">The ID of the company the order belongs to</param>
    /// <param name="request">The order details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ApiResponse containing the created order details</returns>
    public async Task<ApiResponse<OrderResponse>> CreateNewOrderAsync(Guid userId, Guid companyId, OrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Starting order creation process for user {UserId} in company {CompanyId}", userId, companyId);
            var existingUser = await userService.ExistsInCompanyAsync(userId, companyId, cancellationToken);

            if (!existingUser.Data)
            {
                logger.LogWarning(message: "User {userId} does not exist in company {companyId}", userId, companyId);

                return new ApiResponse<OrderResponse>(data: null, HttpStatusCode.BadRequest, TextConstants.UserNotInCompany);
            }

            var modules = await PopulateModules(request, cancellationToken);

            if (modules.Data is null)
            {
                return new ApiResponse<OrderResponse>(data: null, modules.Status, modules.Message);
            }

            if (modules.Data.Count == 0)
            {
                logger.LogWarning(message: "There was an error searching modules");

                return new ApiResponse<OrderResponse>(data: null, HttpStatusCode.BadRequest, TextConstants.ThereWasAnErrorSearchingModules);
            }

            var totalAmount = modules.Data.Sum(m => m.Amount);

            var details = modules.Data.Select(m => new OrderDetailModel { ModuleId = m.Id, Amount = m.Amount }).ToList();

            var order = new OrderModel
            {
                SaleDate = DateTime.UtcNow,
                Status = OrderStatus.Approved,
                UserId = userId,
                TotalAmount = totalAmount,
                Details = details,
                CompanyId = companyId
            };

            await orderRepository.SaveOrderAsync(order, cancellationToken);
            await subscriptionService.CreateCreditsForOrderAsync(order, details, companyId, cancellationToken);

            var response = mapper.Map<OrderResponse>(order);

            logger.LogInformation(message: "Order created successfully for user {UserId}", userId);
            return new ApiResponse<OrderResponse>(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error creating order for user {UserId} in company {CompanyId}", userId, companyId);
            return new ApiResponse<OrderResponse>(data: null, HttpStatusCode.InternalServerError, message: "An error occurred while creating the order");
        }
    }

    /// <summary>
    ///     Populates the module list for the order, including the basic module if necessary
    /// </summary>
    /// <param name="request">The order request containing module details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ApiResponse containing the list of modules</returns>
    private async Task<ApiResponse<List<ModuleModel>>> PopulateModules(OrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogDebug(message: "Starting to populate modules for order request");
            var uniqueModules = request.Details.Select(d => d.ModuleId).Distinct();
            var modules = await moduleService.GetModulesToOrderAsync(uniqueModules, cancellationToken);

            if (modules.Data is null)
            {
                return new ApiResponse<List<ModuleModel>>(data: null, modules.Status, modules.Message);
            }

            if (modules.Data.Any(m => m.Type == ModuleType.Basic))
            {
                var response = mapper.Map<List<ModuleModel>>(modules);

                return new ApiResponse<List<ModuleModel>>(response);
            }

            var basicModule = await moduleService.GetModuleByTypeAsync(ModuleType.Basic, cancellationToken);

            if (basicModule.Data is null)
            {
                return new ApiResponse<List<ModuleModel>>([]);
            }

            modules.Data.Add(basicModule.Data);

            var finalResponse = mapper.Map<List<ModuleModel>>(modules.Data);

            logger.LogInformation(message: "Modules populated successfully");
            return new ApiResponse<List<ModuleModel>>(finalResponse);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error populating modules for order request");
            return new ApiResponse<List<ModuleModel>>(data: null, HttpStatusCode.InternalServerError, message: "An error occurred while populating modules");
        }
    }
}
