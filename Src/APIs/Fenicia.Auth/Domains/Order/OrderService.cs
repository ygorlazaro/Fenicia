using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Order.DTOs;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.Order;

public class OrderService(ModuleService moduleService, OrderRepository orderRepository, SubscriptionService subscriptionService, UserRoleService userRoleService)
{
    public async Task<CreateNewOrderResponse?> CreateAsync(CreateNewOrderCommand command, CancellationToken ct)
    {
        await ValidateUserAsync(command, ct);

        var modules = await PopulateModules(command.Modules, ct);

        if (modules.Count == 0)
        {
            throw new ItemNotExistsException(ExceptionMessages.ModulesNotFound);
        }

        var order = PersistOrderAsync(command, modules);
        await orderRepository.InsertAsync(order, ct);

        LoadCreditsAsync(command.CompanyId, order);
        await subscriptionService.CreateSubscriptionAsync(order.Subscription!, ct);

        return order.MapToCreateNewOrderResponse();
    }

    private static string GenerateOrderNumber()
    {
        return $"AO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
    }

    private OrderModel PersistOrderAsync(CreateNewOrderCommand command, List<ModuleModel> modules)
    {
        var totalAmount = modules.Sum(m => m.Price);
        var orderNumber = GenerateOrderNumber();

        var details = modules.Select(m => new OrderDetailModel
        {
            ModuleId = m.Id,
            Price = m.Price
        }).ToList();

        var order = new OrderModel
        {
            OrderNumber = orderNumber,
            SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Approved,
            UserId = command.UserId,
            TotalAmount = totalAmount,
            Details = details,
            CompanyId = command.CompanyId
        };

        return order;
    }

    private async Task ValidateUserAsync(CreateNewOrderCommand command, CancellationToken ct)
    {
        var existingUser = await userRoleService.AnyIdAndCompanyAsync(command.UserId, command.CompanyId, ct);

        if (!existingUser)
        {
            throw new PermissionDeniedException(ExceptionMessages.UserDoesNotExistsAtCompany);
        }
    }

    private async Task<List<ModuleModel>> PopulateModules(List<Guid> request, CancellationToken ct)
    {
        try
        {
            var modules = await GetModulesToOrderAsync(request.Distinct(), ct);

            if (modules.Any(m => m.Type == ModuleType.Basic))
            {
                return modules;
            }

            var basicModule = await GetModuleByTypeAsync(ModuleType.Basic, ct);

            return basicModule switch
            {
                null => [],
                _ => [basicModule, .. modules]
            };
        }
#pragma warning disable CA1031
        catch
        {
            return [];
        }
#pragma warning restore CA1031
    }

    private async Task<List<ModuleModel>> GetModulesToOrderAsync(IEnumerable<Guid> request, CancellationToken ct)
    {
        return await moduleService.GetModulesByIdsAsync(request, ct);
    }

    private async Task<ModuleModel?> GetModuleByTypeAsync(ModuleType moduleType, CancellationToken ct)
    {
        return await moduleService.GetModuleByTypeAsync(moduleType, ct);
    }

    private void LoadCreditsAsync(Guid companyId, OrderModel order)
    {
        var credits = order.Details.Select(d => new SubscriptionCreditModel
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

        order.Subscription = subscription;
    }
}
