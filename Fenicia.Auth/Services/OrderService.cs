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
    ICustomerService customerService,
    IModuleService moduleService,
    ISubscriptionService subscriptionService,
    IUserService userService
) : IOrderService
{
    public async Task<OrderResponse> CreateNewOrderAsync(Guid userId, Guid companyId, NewOrderRequest request)
    {
        logger.LogInformation("Creating new order");
        var existingUser = await userService.ExistsInCompanyAsync(userId, companyId);

        if (!existingUser)
        {
            logger.LogWarning("User {userId} does not exist in company {companyId}", [userId, companyId]);
            throw new UnauthorizedAccessException(TextConstants.PermissionDenied);
        }

        var customerId = await customerService.GetOrCreateByUserIdAsync(userId, companyId);

        if (customerId is null)
        {
            logger.LogWarning("There was an error creating customer for user {userId}", [userId]);
            throw new ArgumentException(TextConstants.ThereWasAnErrorAtCreatingCustomer);
        }

        var modules = await PopulateModules(request);

        if (modules.Count == 0)
        {
            logger.LogWarning("There was an error searching modules");
            throw new InvalidDataException(TextConstants.ThereWasAnErrorSearchingModules);
        }

        var totalAmount = modules.Sum(m => m.Amount);

        var details = modules.Select(m => new OrderDetailModel
        {
            ModuleId = m.Id,
            Amount = m.Amount
        }).ToList();

        var order = new OrderModel
        {
            SaleDate = DateTime.Now,
            Status = OrderStatus.Approved,
            CustomerId = customerId.Value,
            TotalAmount = totalAmount,
            Details = details
        };

        await orderRepository.SaveOrderAsync(order);
        await subscriptionService.CreateCreditsForOrderAsync(order, details, companyId);
        
        var response = mapper.Map<OrderResponse>(order);

        return response;
    }

    private async Task<List<ModuleModel>> PopulateModules(NewOrderRequest request)
    {
        var uniqueModules = request.Details.Select(d => d.ModuleId).Distinct();
        var modules = await moduleService.GetModulesToOrderAsync(uniqueModules);

        if (modules.Any(m => m.Type == ModuleType.Basic))
        {
            return mapper.Map<List<ModuleModel>>(modules);
        }

        var basicModule = await moduleService.GetModuleByTypeAsync(ModuleType.Basic);

        if (basicModule is null)
        {
            return [];
        }
        
        modules.Add(basicModule);
        
        return  mapper.Map<List<ModuleModel>>(modules);
    }
}