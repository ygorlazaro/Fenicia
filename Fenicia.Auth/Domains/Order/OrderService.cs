using Fenicia.Auth.Domains.Module.Interfaces;
using Fenicia.Auth.Domains.Order.Interfaces;
using Fenicia.Auth.Domains.Subscription.Interfaces;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Order;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.Order;

public class OrderService(
    IOrderRepository repository,
    IModuleService moduleService,
    ISubscriptionService subscriptionService,
    IUserRoleService userRoleService) : IOrderService
{
    public async Task<OrderResponse?> CreateAsync(
        OrderRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidateUserAsync(request, cancellationToken);

        var modules = await PopulateModules(request.Modules, cancellationToken);

        if (modules.Count == 0)
        {
            throw new ForbiddenException(ExceptionMessages.ModulesNotFound);
        }

        var order = PersistOrderAsync(request, modules);
        await repository.InsertAsync(order, cancellationToken);

        LoadCreditsAsync(request.CompanyId, order);
        await subscriptionService.CreateSubscriptionAsync(order.Subscription!, cancellationToken);

        return new OrderResponse(order.Id);
    }

    private static string GenerateOrderNumber()
    {
        return $"AO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
    }

    private static void LoadCreditsAsync(Guid companyId, OrderModel order)
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
            Status = EnumSubscriptionStatus.Active,
            CompanyId = companyId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            OrderId = order.Id,
            Credits = credits
        };

        order.Subscription = subscription;
    }

    private static OrderModel PersistOrderAsync(OrderRequest request, List<ModuleModel> modules)
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
            Status = EnumOrderStatus.Approved,
            UserId = request.UserId,
            TotalAmount = totalAmount,
            Details = details,
            CompanyId = request.CompanyId
        };

        return order;
    }

    private async Task ValidateUserAsync(OrderRequest request, CancellationToken cancellationToken = default)
    {
        var existingUser = await userRoleService.AnyIdAndCompanyAsync(
            request.UserId,
            request.CompanyId,
            cancellationToken);

        if (!existingUser)
        {
            throw new PermissionDeniedException(ExceptionMessages.UserDoesNotExistsAtCompany);
        }
    }

    private async Task<List<ModuleModel>> PopulateModules(
        IEnumerable<Guid> request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var modules = await GetModulesToOrderAsync(request.Distinct(), cancellationToken);

            if (modules.Any(m => m.Type == EnumModuleType.Basic))
            {
                return modules;
            }

            var basicModule = await GetModuleByTypeAsync(EnumModuleType.Basic, cancellationToken);

            return basicModule switch
            {
                null => [],
                _ => [basicModule, .. modules]
            };
        }
        catch (BadRequestException)
        {
            return [];
        }
        catch (ForbiddenException)
        {
            return [];
        }
    }

    private Task<List<ModuleModel>> GetModulesToOrderAsync(
        IEnumerable<Guid> request,
        CancellationToken cancellationToken = default)
    {
        return moduleService.GetModulesByIdsAsync(request, cancellationToken);
    }

    private Task<ModuleModel?> GetModuleByTypeAsync(
        EnumModuleType moduleType,
        CancellationToken cancellationToken = default)
    {
        return moduleService.GetModuleByTypeAsync(moduleType, cancellationToken);
    }
}
