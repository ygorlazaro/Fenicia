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

public class CreateNewOrderHandler(DefaultContext db)
{
    public virtual async Task<CreateNewOrderResponse?> Handle(CreateNewOrderCommand command, CancellationToken ct)
    {
        await ValidateUserAsync(command, ct);

        var modules = await PopulateModules(command.Modules, ct);

        if (modules.Count == 0)
        {
            throw new ItemNotExistsException(ExceptionMessages.ModulesNotFound);
        }

        var order = await PersistOrderAsync(command, modules, ct);

        await LoadCreditsAsync(order.Id, command.CompanyId, order.Details, ct);
        
        return new CreateNewOrderResponse(order.Id);
    }

    private async Task<OrderModel> PersistOrderAsync(CreateNewOrderCommand command, List<ModuleModel> modules, CancellationToken ct)
    {
        var totalAmount = modules.Sum(m => m.Price);
        
        var details = modules.Select(m => new OrderDetailModel
        {
            ModuleId = m.Id,
            Price = m.Price
        }).ToList();
        
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

        await db.SaveChangesAsync(ct);
        
        return order;
    }

    private async Task ValidateUserAsync(CreateNewOrderCommand command, CancellationToken ct)
    {
        var existingUser = await db.AuthUserRoles.AnyIdAndCompanyAsync(command.UserId, command.CompanyId, ct);

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

            if (basicModule is null)
            {
                return [];
            }

            return [basicModule, ..modules];
        }
        catch
        {
            return [];
        }
    }

    private async Task<List<ModuleModel>> GetModulesToOrderAsync(IEnumerable<Guid> request, CancellationToken ct)
    {
        return await db.AuthModules.Where(module => request.Any(r => r == module.Id))
            .OrderBy(module => module.Type)
            .ToListAsync(ct);
    }

    private async Task<ModuleModel?> GetModuleByTypeAsync(ModuleType moduleType, CancellationToken ct)
    {
        return await db.AuthModules.FirstOrDefaultAsync(m => m.Type == moduleType, ct);
    }

    private async Task LoadCreditsAsync(Guid orderId, Guid companyId, List<OrderDetailModel> details, CancellationToken ct)
    {
        var credits = details.Select(d => new SubscriptionCreditModel
        {
            ModuleId = d.ModuleId,
            IsActive = true,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            OrderDetailId = d.Id
        }).ToList();

        var subscription = new SubscriptionModel
        {
            Status = SubscriptionStatus.Active,
            CompanyId = companyId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            OrderId = orderId,
            Credits = credits
        };

        db.AuthSubscriptions.Add(subscription);

        await db.SaveChangesAsync(ct);
    }
}
