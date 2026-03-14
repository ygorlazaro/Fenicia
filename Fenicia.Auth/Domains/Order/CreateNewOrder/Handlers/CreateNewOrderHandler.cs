using Fenicia.Auth.Domains.Order.CreateNewOrder.Commands;
using Fenicia.Auth.Domains.Order.CreateNewOrder.Responses;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Order.CreateNewOrder.Handlers;

/// <summary>
/// Handler responsible for creating new module subscription orders.
/// Processes orders by validating user, populating modules, and creating subscriptions.
/// </summary>
/// <remarks>
/// This handler implements the business logic for module subscription orders:
/// 1. Validates user has a role in the company
/// 2. Populates requested modules (deduplicates, adds Basic if needed)
/// 3. Creates order with Approved status
/// 4. Creates 1-month subscription with active credits
/// 
/// Related documentation:
/// - See <see cref="Fenicia.Auth.Domains.Module.Queries.GetModulesQuery"/> for module queries
/// - See <see cref="Fenicia.Auth.Domains.UserRole.UserRoleExtensions"/> for user-company validation
/// </remarks>
public class CreateNewOrderHandler(DefaultContext db)
{
    /// <summary>
    /// Creates a new order and associated subscription for module subscriptions.
    /// </summary>
    /// <param name="command">The command containing user ID, company ID, and module IDs.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created order ID, or null if no modules requested.</returns>
    /// <exception cref="PermissionDeniedException">Thrown when user does not belong to the company.</exception>
    /// <exception cref="ItemNotExistsException">Thrown when requested modules or Basic module not found.</exception>
    public virtual async Task<CreateNewOrderResponse?> Handle(CreateNewOrderCommand command, CancellationToken ct)
    {
        await ValidateUserAsync(command, ct);

        var modules = await PopulateModules(command.Modules, ct);

        if (modules.Count == 0)
        {
            throw new ItemNotExistsException(ExceptionMessages.ModulesNotFound);
        }

        var order = PersistOrderAsync(command, modules);

        LoadCreditsAsync(command.CompanyId, order);

        await db.SaveChangesAsync(ct);

        return new CreateNewOrderResponse(order.Id);
    }

    /// <summary>
    /// Persists the order and its details to the database.
    /// </summary>
    /// <param name="command">The create order command.</param>
    /// <param name="modules">The list of modules to include in the order.</param>
    /// <returns>The created order model.</returns>
    private OrderModel PersistOrderAsync(CreateNewOrderCommand command, List<ModuleModel> modules)
    {
        var totalAmount = modules.Sum(m => m.Price);
        
        var details = modules.Select(m => new OrderDetailModel
        {
            ModuleId = m.Id,
            Price = m.Price
        });

        var order = new OrderModel
        {
            SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Approved,
            UserId = command.UserId,
            TotalAmount = totalAmount,
            Details = details,
            CompanyId = command.CompanyId
        };

        db.AuthOrders.Add(order);

        return order;
    }

    /// <summary>
    /// Validates that the user has a role in the specified company.
    /// </summary>
    /// <param name="command">The create order command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <exception cref="PermissionDeniedException">Thrown when user does not belong to the company.</exception>
    private async Task ValidateUserAsync(CreateNewOrderCommand command, CancellationToken ct)
    {
        var existingUser = await db.AuthUserRoles.AnyIdAndCompanyAsync(command.UserId, command.CompanyId, ct);

        if (!existingUser)
        {
            throw new PermissionDeniedException(ExceptionMessages.UserDoesNotExistsAtCompany);
        }
    }

    /// <summary>
    /// Populates the list of modules to order, handling deduplication and automatic Basic module inclusion.
    /// </summary>
    /// <param name="request">List of module IDs requested.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of modules to order, or empty list if validation fails.</returns>
    /// <remarks>
    /// If the requested modules don't include Basic, it will be automatically added.
    /// If Basic module doesn't exist in the database, returns empty list.
    /// Duplicates in the request are removed.
    /// </remarks>
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
        catch
        {
            return [];
        }
    }

    /// <summary>
    /// Retrieves modules from the database based on requested IDs.
    /// </summary>
    /// <param name="request">List of module IDs.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of found modules ordered by type.</returns>
    private async Task<List<ModuleModel>> GetModulesToOrderAsync(IEnumerable<Guid> request, CancellationToken ct)
    {
        return await db.AuthModules.Where(module => request.Any(r => r == module.Id))
            .OrderBy(module => module.Type)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Retrieves a module by its type.
    /// </summary>
    /// <param name="moduleType">The module type to search for.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The module if found, otherwise null.</returns>
    private async Task<ModuleModel?> GetModuleByTypeAsync(ModuleType moduleType, CancellationToken ct)
    {
        return await db.AuthModules.FirstOrDefaultAsync(m => m.Type == moduleType, ct);
    }

    /// <summary>
    /// Creates subscription credits for each ordered module and adds the subscription to the database.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <param name="order">The order to create credits for.</param>
    /// <remarks>
    /// Creates a 1-month subscription starting from the current UTC time.
    /// Each module gets an active credit with the same validity period.
    /// </remarks>
    private void LoadCreditsAsync(Guid companyId, OrderModel order)
    {
        var credits = order.Details.Select(d => new SubscriptionCreditModel
        {
            ModuleId = d.ModuleId,
            IsActive = true,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            OrderDetailId = d.Id
        });

        var subscription = new SubscriptionModel
        {
            Status = SubscriptionStatus.Active,
            CompanyId = companyId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            OrderId = order.Id,
            Credits = credits
        };

        db.AuthSubscriptions.Add(subscription);
    }
}
