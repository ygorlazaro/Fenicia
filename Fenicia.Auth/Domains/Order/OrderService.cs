using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Module.Interfaces;
using Fenicia.Auth.Domains.Order.Interfaces;
using Fenicia.Auth.Domains.Subscription.Interfaces;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Module;
using Fenicia.Common.DTOs.Auth.Order;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.Order;

/// <summary>
/// Service responsible for managing orders, including creation and validation of orders.
/// </summary>
/// <param name="repository">The order repository.</param>
/// <param name="moduleService">The module service.</param>
/// <param name="subscriptionService">The subscription service.</param>
/// <param name="userRoleService">The user role service.</param>
public class OrderService(
    IOrderRepository repository,
    IModuleService moduleService,
    ISubscriptionService subscriptionService,
    IUserRoleService userRoleService) : IOrderService
{
    /// <summary>
    /// Creates a new order based on the provided request. Validates the user, populates the modules, and persists the order in the repository. Also creates a subscription for the order.
    /// </summary>
    /// <param name="request">The order request containing user and module information.</param>
    /// <param name="cancellationToken">A cancellation token for the operation.</param>
    /// <returns>An OrderResponse containing the ID of the created order, or null if the order could not be created.</returns>
    /// <exception cref="ForbiddenException">Thrown when no valid modules are found for the order.</exception>
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

    /// <summary>
    /// Generates a unique order number based on the current date and a random GUID.
    /// </summary>
    /// <returns>A unique order number string.</returns>
    private static string GenerateOrderNumber()
    {
        return $"AO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
    }

    /// <summary>
    /// Loads credits for the order based on its details.
    /// </summary>
    /// <param name="companyId">The ID of the company.</param>
    /// <param name="order">The order for which to load credits.</param>
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

    /// <summary>
    /// Persists the order in the repository.
    /// </summary>
    /// <param name="request">The order request containing user and module information.</param>
    /// <param name="modules">The list of modules for the order.</param>
    /// <returns>The persisted order model.</returns>
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

    /// <summary>
    /// Validates if the user exists and is associated with the specified company. Throws a PermissionDeniedException if the user does not exist or is not associated with the company.
    /// </summary>
    /// <param name="request">The order request containing user and module information.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="PermissionDeniedException"></exception>
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

    /// <summary>
    /// Populates the list of modules for the order.
    /// </summary>
    /// <param name="request">The list of module IDs for the order.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task<List<ModuleModel>> PopulateModules(
        IEnumerable<Guid> request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var modules = await GetModulesToOrderAsync(request.Distinct(), cancellationToken);

            if (modules.Any(m => m.Type == EnumModuleType.Basic))
            {
                return [.. modules.Select(ModuleMapper.MapToModuleModel)];
            }

            var basicModule = await GetModuleByTypeAsync(EnumModuleType.Basic, cancellationToken);

            if (basicModule is null)
            {
                return [];
            }

            modules.Add(basicModule);

            return [.. modules.Select(ModuleMapper.MapToModuleModel)];
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

    private async Task<List<ModuleResponse>> GetModulesToOrderAsync(
        IEnumerable<Guid> request,
        CancellationToken cancellationToken = default)
    {
        var modules = await moduleService.GetModulesByIdsAsync(request, cancellationToken);

        return [.. modules];
    }

    private Task<ModuleResponse?> GetModuleByTypeAsync(
        EnumModuleType moduleType,
        CancellationToken cancellationToken = default)
    {
        return moduleService.GetModuleByTypeAsync(moduleType, cancellationToken);
    }
}
