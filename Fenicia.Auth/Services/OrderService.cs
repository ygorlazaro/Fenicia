using System.Net;
using AutoMapper;
using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Enums;
using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Requests;
using Fenicia.Auth.Responses;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Services;

public class OrderService(
    IMapper mapper,
    ILogger<OrderService> logger,
    IOrderRepository orderRepository,
    IModuleService moduleService,
    ISubscriptionService subscriptionService,
    IUserService userService
) : IOrderService
{
    public async Task<ServiceResponse<OrderResponse>> CreateNewOrderAsync(
        Guid userId,
        Guid companyId,
        OrderRequest request
    )
    {
        logger.LogInformation("Creating new order");
        var existingUser = await userService.ExistsInCompanyAsync(userId, companyId);

        if (!existingUser.Data)
        {
            logger.LogWarning(
                "User {userId} does not exist in company {companyId}",
                [userId, companyId]
            );

            return new ServiceResponse<OrderResponse>(
                null,
                HttpStatusCode.BadRequest,
                TextConstants.UserNotInCompany
            );
        }

        var modules = await PopulateModules(request);

        if (modules.Data is null)
        {
            return new ServiceResponse<OrderResponse>(null, modules.StatusCode, modules.Message);
        }

        if (modules.Data.Count == 0)
        {
            logger.LogWarning("There was an error searching modules");

            return new ServiceResponse<OrderResponse>(
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
            SaleDate = DateTime.Now,
            Status = OrderStatus.Approved,
            UserId = userId,
            TotalAmount = totalAmount,
            Details = details,
        };

        await orderRepository.SaveOrderAsync(order);
        await subscriptionService.CreateCreditsForOrderAsync(order, details, companyId);

        var response = mapper.Map<OrderResponse>(order);

        return new ServiceResponse<OrderResponse>(response);
    }

    private async Task<ServiceResponse<List<ModuleModel>>> PopulateModules(OrderRequest request)
    {
        var uniqueModules = request.Details.Select(d => d.ModuleId).Distinct();
        var modules = await moduleService.GetModulesToOrderAsync(uniqueModules);

        if (modules.Data is null)
        {
            return new ServiceResponse<List<ModuleModel>>(
                null,
                modules.StatusCode,
                modules.Message
            );
        }

        if (modules.Data.Any(m => m.Type == ModuleType.Basic))
        {
            var response = mapper.Map<List<ModuleModel>>(modules);

            return new ServiceResponse<List<ModuleModel>>(response);
        }

        var basicModule = await moduleService.GetModuleByTypeAsync(ModuleType.Basic);

        if (basicModule.Data is null)
        {
            return new ServiceResponse<List<ModuleModel>>([]);
        }

        modules.Data.Add(basicModule.Data);

        var finalResponse = mapper.Map<List<ModuleModel>>(modules.Data);

        return new ServiceResponse<List<ModuleModel>>(finalResponse);
    }
}
