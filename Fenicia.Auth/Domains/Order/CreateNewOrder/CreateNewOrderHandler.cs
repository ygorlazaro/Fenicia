using Fenicia.Auth.Domains.Subscription.CreateCreditsForOrder;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Order.CreateNewOrder;

public class CreateNewOrderHandler(
    DefaultContext db,
    CreateCreditsForOrderHandler createCreditsForOrderHandler)
{
    public virtual async Task<CreateNewOrderResponse?> Handle(CreateNewOrderCommand command, CancellationToken ct)
    {
        var existingUser = await UserExistsAsync(command.UserId, command.CompanyId, ct);

        if (!existingUser)
        {
            throw new PermissionDeniedException(ExceptionMessages.UserDoesNotExistsAtCompany);
        }

        var modules = await PopulateModules(command.Modules, ct);

        if (modules.Count == 0)
        {
            throw new ItemNotExistsException(ExceptionMessages.ModulesNotFound);
        }

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

        await createCreditsForOrderHandler.Handle(
            new CreateCreditsForOrderQuery(order.Id, order.CompanyId,
                order.Details.Select(d => new CreateCreditsForOrderDetailsQuery(d.Id, d.ModuleId))), ct);

        return new CreateNewOrderResponse(order.Id);
    }

    private async Task<bool> UserExistsAsync(Guid userId, Guid companyId, CancellationToken ct)
    {
        var query = from ur in db.AuthUserRoles
                     where ur.CompanyId == companyId
                           && ur.UserId == userId
                     select 1;

        return await query.AnyAsync(ct);
    }

    private async Task<List<ModuleModel>> PopulateModules(List<Guid> request, CancellationToken ct)
    {
        try
        {
            var uniqueModules = request.Distinct();
            var modules = await GetModulesToOrderAsync(uniqueModules, ct);

            if (modules.Any(m => m.Type == ModuleType.Basic))
            {
                return modules;
            }

            var basicModule = await GetModuleByTypeAsync(ModuleType.Basic, ct);

            if (basicModule is null)
            {
                return [];
            }

            modules.Add(basicModule);

            return modules;
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
}
