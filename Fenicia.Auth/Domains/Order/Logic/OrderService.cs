using System.Net;

using AutoMapper;

using Fenicia.Auth.Domains.Module.Data;
using Fenicia.Auth.Domains.Module.Logic;
using Fenicia.Auth.Domains.Order.Data;
using Fenicia.Auth.Domains.OrderDetail.Data;
using Fenicia.Auth.Domains.Subscription.Logic;
using Fenicia.Auth.Domains.User.Logic;
using Fenicia.Auth.Enums;
using Fenicia.Common;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.Order.Logic;

public class OrderService(
    IMapper mapper,
    ILogger<OrderService> logger,
    IOrderRepository orderRepository,
    IModuleService moduleService,
    ISubscriptionService subscriptionService,
    IUserService userService
) : IOrderService
{
    public async Task<ApiResponse<OrderResponse>> CreateNewOrderAsync(Guid userId,
        Guid companyId,
        OrderRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new order");
        var existingUser = await userService.ExistsInCompanyAsync(userId, companyId, cancellationToken);

        if (!existingUser.Data)
        {
            logger.LogWarning(
                "User {userId} does not exist in company {companyId}", userId, companyId);

            return new ApiResponse<OrderResponse>(
                null,
                HttpStatusCode.BadRequest,
                TextConstants.UserNotInCompany
            );
        }

        var modules = await PopulateModules(request, cancellationToken);

        if (modules.Data is null)
        {
            return new ApiResponse<OrderResponse>(null, modules.Status, modules.Message);
        }

        if (modules.Data.Count == 0)
        {
            logger.LogWarning("There was an error searching modules");

            return new ApiResponse<OrderResponse>(
                null,
                HttpStatusCode.BadRequest,
                TextConstants.ThereWasAnErrorSearchingModules
            );
        }

        var totalAmount = modules.Data.Sum(m => m.Amount);

        var details = modules
            .Data.Select(m => new OrderDetailModel { ModuleId = m.Id, Amount = m.Amount })
            .ToList();

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

        return new ApiResponse<OrderResponse>(response);
    }

    private async Task<ApiResponse<List<ModuleModel>>> PopulateModules(OrderRequest request,
        CancellationToken cancellationToken)
    {
        var uniqueModules = request.Details.Select(d => d.ModuleId).Distinct();
        var modules = await moduleService.GetModulesToOrderAsync(uniqueModules, cancellationToken);

        if (modules.Data is null)
        {
            return new ApiResponse<List<ModuleModel>>(
                null,
                modules.Status,
                modules.Message
            );
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

        return new ApiResponse<List<ModuleModel>>(finalResponse);
    }
}
