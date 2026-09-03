using Fenicia.Auth.Domains.Module.Interfaces;
using Fenicia.Auth.Domains.Order.DTOs;
using Fenicia.Auth.Domains.Order.Interfaces;
using Fenicia.Auth.Domains.Subscription.Interfaces;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.Order;

public class OrderService(
    IModuleService moduleService,
    IRepository<OrderModel> orderRepository,
    ISubscriptionService subscriptionService,
    IUserRoleService userRoleService) : IOrderService
{
    public async Task<CreateNewOrderResponse?> CreateAsync(
        CreateNewOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        await ValidateUserAsync(command, cancellationToken);

        var modules = await PopulateModules(command.Modules, cancellationToken);

        if (modules.Count == 0)
        {
            throw new ItemNotExistsException(ExceptionMessages.ModulesNotFound);
        }

        var order = PersistOrderAsync(command, modules);
        await orderRepository.InsertAsync(order, cancellationToken);

        LoadCreditsAsync(command.CompanyId, order);
        await subscriptionService.CreateSubscriptionAsync(order.Subscription!, cancellationToken);

        return order.MapToCreateNewOrderResponse();
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
            Status = SubscriptionStatus.Active,
            CompanyId = companyId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            OrderId = order.Id,
            Credits = credits
        };

        order.Subscription = subscription;
    }

    private static OrderModel PersistOrderAsync(CreateNewOrderCommand command, List<ModuleModel> modules)
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

    private async Task ValidateUserAsync(CreateNewOrderCommand command, CancellationToken cancellationToken = default)
    {
        var existingUser = await userRoleService.AnyIdAndCompanyAsync(
            command.UserId,
            command.CompanyId,
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

            if (modules.Any(m => m.Type == ModuleType.Basic))
            {
                return modules;
            }

            var basicModule = await GetModuleByTypeAsync(ModuleType.Basic, cancellationToken);

            return basicModule switch
            {
                null => [],
                _ => [basicModule, .. modules]
            };
        }
        catch (InvalidRequestException)
        {
            return [];
        }
        catch (ItemNotExistsException)
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
        ModuleType moduleType,
        CancellationToken cancellationToken = default)
    {
        return moduleService.GetModuleByTypeAsync(moduleType, cancellationToken);
    }
}