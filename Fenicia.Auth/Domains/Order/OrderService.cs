using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.User;
using Fenicia.Common;
using Fenicia.Common.Data.Mappers.Auth;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Migrations.Services;

using OrderMapper = Fenicia.Common.Data.Mappers.Auth.OrderMapper;

namespace Fenicia.Auth.Domains.Order;

public sealed class OrderService(IOrderRepository orderRepository, IModuleService moduleService, ISubscriptionService subscriptionService, IUserService userService, IMigrationService migrationService)
    : IOrderService
{
    public async Task<OrderResponse?> CreateNewOrderAsync(Guid userId, Guid companyId, OrderRequest request, CancellationToken ct)
    {
        var existingUser = await userService.ExistsInCompanyAsync(userId, companyId, ct);

        if (!existingUser)
        {
            throw new PermissionDeniedException(TextConstants.UserDoestNotExistsAtTheCompany);
        }

        var modules = await PopulateModules(request, ct) ?? throw new ItemNotExistsException(TextConstants.ModulesNotFound);

        if (modules.Count == 0)
        {
            return null;
        }

        var totalAmount = modules.Sum(m => m.Price);
        var details = modules.Select(m => new OrderDetailModel { ModuleId = m.Id, Price = m.Price }).ToList();
        var order = new OrderModel
        {
            SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Approved,
            UserId = userId,
            TotalAmount = totalAmount,
            Details = details,
            CompanyId = companyId
        };

        orderRepository.Add(order);

        await subscriptionService.CreateCreditsForOrderAsync(order, details, companyId, ct);

        await orderRepository.SaveChangesAsync(ct);
        await migrationService.RunMigrationsAsync(companyId, [.. modules.Select(m => m.Type)], ct);

        return OrderMapper.Map(order);
    }

    private async Task<List<ModuleModel>?> PopulateModules(OrderRequest request, CancellationToken ct)
    {
        try
        {
            var uniqueModules = request.Details.Select(d => d.ModuleId).Distinct();
            var modules = await moduleService.GetModulesToOrderAsync(uniqueModules, ct);

            if (modules.Any(m => m.Type == ModuleType.Basic))
            {
                return ModuleMapper.Map(modules);
            }

            var basicModule = await moduleService.GetModuleByTypeAsync(ModuleType.Basic, ct);

            if (basicModule is null)
            {
                return null;
            }

            modules.Add(basicModule);

            return ModuleMapper.Map(modules);
        }
        catch
        {
            return null;
        }
    }
}