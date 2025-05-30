using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Enums;
using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Requests;
using Fenicia.Auth.Services.Interfaces;

namespace Fenicia.Auth.Services;

public class OrderService(
    IOrderRepository orderRepository,
    ICustomerService customerService,
    IModuleService moduleService,
    ISubscriptionService subscriptionService,
    IUserService userService
) : IOrderService
{
    public async Task<OrderModel?> CreateNewOrderAsync(Guid userId, Guid companyId, NewOrderRequest request)
    {
        var existingUser = await userService.ExistsInCompanyAsync(userId, companyId);

        if (!existingUser)
        {
            return null;
        }

        var customerId = await customerService.GetOrCreateByUserIdAsync(userId, companyId);

        if (customerId is null)
        {
            Console.WriteLine("Houve um problema criando ou buscando o cliente");

            return null;
        }

        var modules = await PopulateModules(request);

        if (modules.Count == 0)
        {
            Console.WriteLine("Houve um problema buscando os módulos");

            return null;
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

        return order;
    }

    private async Task<List<ModuleModel>> PopulateModules(NewOrderRequest request)
    {
        var uniqueModules = request.Details.Select(d => d.ModuleId).Distinct();
        var modules = await moduleService.GetModulesToOrderAsync(uniqueModules);

        if (modules.Any(m => m.Type == ModuleType.Basic))
        {
            return modules;
        }

        var basicModule = await moduleService.GetModuleByTypeAsync(ModuleType.Basic);

        if (basicModule is null)
        {
            return [];
        }

        return [basicModule, ..modules];
    }
}