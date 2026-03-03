using Fenicia.Auth.Domains.Subscription.CreateCreditsForOrder;
using Fenicia.Auth.Domains.User;
using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Order.CreateNewOrder;

public class CreateNewOrderHandler(
    DefaultContext db,
    CreateCreditsForOrderHandler createCreditsForOrderHandler)
{
    public virtual async Task<CreateNewOrderResponse?> Handle(CreateNewOrderCommand command, CancellationToken ct)
    {
        var existingUser = await db.UserExistsAsync(command.UserId, command.CompanyId, ct);

        if (!existingUser)
        {
            throw new PermissionDeniedException(TextConstants.UserDoestNotExistsAtTheCompany);
        }

        var modules = await PopulateModules(command.Modules, ct);

        if (modules.Count == 0)
        {
            throw new ItemNotExistsException(TextConstants.ModulesNotFound);
        }

        var totalAmount = modules.Sum(m => m.Price);
        var details = modules.Select(m => new AuthOrderDetailModel { ModuleId = m.Id, Price = m.Price }).ToList();
        var order = new AuthOrderModel
        {
            SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Approved,
            UserId = command.UserId,
            TotalAmount = totalAmount,
            Details = details,
            CompanyId = command.CompanyId
        };

        db.Orders.Add(order);

        await db.SaveChangesAsync(ct);

        await createCreditsForOrderHandler.Handle(
            new CreateCreditsForOrderQuery(order.Id, order.CompanyId,
                order.Details.Select(d => new CreateCreditsForOrderDetailsQuery(d.Id, d.ModuleId))), ct);

        return new CreateNewOrderResponse(order.Id);
    }

    private async Task<List<AuthModuleModel>> PopulateModules(List<Guid> request, CancellationToken ct)
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

    private async Task<List<AuthModuleModel>> GetModulesToOrderAsync(IEnumerable<Guid> request, CancellationToken ct)
    {
        return await db.Modules.Where(module => request.Any(r => r == module.Id))
            .OrderBy(module => module.Type)
            .ToListAsync(ct);
    }

    private async Task<AuthModuleModel?> GetModuleByTypeAsync(ModuleType moduleType, CancellationToken ct)
    {
        return await db.Modules.FirstOrDefaultAsync(m => m.Type == moduleType, ct);
    }
}
