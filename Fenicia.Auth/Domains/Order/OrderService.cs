using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.User;
using Fenicia.Common;
using Fenicia.Common.Data.Converters.Auth;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;
using Fenicia.Common.Enums;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Migrations.Services;

namespace Fenicia.Auth.Domains.Order;

public sealed class OrderService(IOrderRepository orderRepository, IModuleService moduleService, ISubscriptionService subscriptionService, IUserService userService, IMigrationService migrationService)
    : IOrderService
{
    public async Task<OrderResponse?> CreateNewOrderAsync(Guid userId, Guid companyId, OrderRequest request, CancellationToken cancellationToken)
    {
        var existingUser = await userService.ExistsInCompanyAsync(userId, companyId, cancellationToken);

        if (!existingUser)
        {
            throw new PermissionDeniedException(TextConstants.UserDoestNotExistsAtTheCompany);
        }

        var modules = await PopulateModules(request, cancellationToken) ?? throw new ItemNotExistsException(TextConstants.ModulesNotFound);

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

        await subscriptionService.CreateCreditsForOrderAsync(order, details, companyId, cancellationToken);

        await orderRepository.SaveChangesAsync(cancellationToken);
        await migrationService.RunMigrationsAsync(companyId, [.. modules.Select(m => m.Type)], cancellationToken);

        return OrderResponse.Convert(order);
    }

    private async Task<List<ModuleModel>?> PopulateModules(OrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var uniqueModules = request.Details.Select(d => d.ModuleId).Distinct();
            var modules = await moduleService.GetModulesToOrderAsync(uniqueModules, cancellationToken);

            if (modules.Any(m => m.Type == ModuleType.Basic))
            {
                return ModuleConverter.Convert(modules);
            }

            var basicModule = await moduleService.GetModuleByTypeAsync(ModuleType.Basic, cancellationToken);

            if (basicModule is null)
            {
                return null;
            }

            modules.Add(basicModule);

            return ModuleConverter.Convert(modules);
        }
        catch
        {
            return null;
        }
    }
}
