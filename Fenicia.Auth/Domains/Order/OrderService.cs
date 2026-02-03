using System.Net;

using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.User;
using Fenicia.Common;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.Order;

public sealed class OrderService(IOrderRepository orderRepository, IModuleService moduleService, ISubscriptionService subscriptionService, IUserService userService)
    : IOrderService
{
    public async Task<ApiResponse<OrderResponse>> CreateNewOrderAsync(Guid userId, Guid companyId, OrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var existingUser = await userService.ExistsInCompanyAsync(userId, companyId, cancellationToken);

            if (!existingUser.Data)
            {
                return new ApiResponse<OrderResponse>(data: null, HttpStatusCode.BadRequest, TextConstants.UserNotInCompanyMessage);
            }

            var modules = await PopulateModules(request, cancellationToken);

            if (modules.Data is null)
            {
                return new ApiResponse<OrderResponse>(data: null, modules.Status, modules.Message?.Message ?? string.Empty);
            }

            if (modules.Data.Count == 0)
            {
                return new ApiResponse<OrderResponse>(data: null, HttpStatusCode.BadRequest, TextConstants.ThereWasAnErrorSearchingModulesMessage);
            }

            var totalAmount = modules.Data.Sum(m => m.Amount);
            var details = modules.Data.Select(m => new OrderDetailModel { ModuleId = m.Id, Amount = m.Amount }).ToList();
            var order = new OrderModel
            {
                SaleDate = DateTime.UtcNow,
                Status = OrderStatus.Approved,
                UserId = userId,
                TotalAmount = totalAmount,
                Details = details,
                CompanyId = companyId
            };

            await orderRepository.SaveOrderAsync(order, cancellationToken);
            await subscriptionService.CreateCreditsForOrderAsync(order, details, companyId, cancellationToken);

            var response = OrderResponse.Convert(order);

            return new ApiResponse<OrderResponse>(response);
        }
        catch
        {
            return new ApiResponse<OrderResponse>(data: null, HttpStatusCode.InternalServerError, "An error occurred while creating the order");
        }
    }

    private async Task<ApiResponse<List<ModuleModel>>> PopulateModules(OrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var uniqueModules = request.Details.Select(d => d.ModuleId).Distinct();
            var modules = await moduleService.GetModulesToOrderAsync(uniqueModules, cancellationToken);

            if (modules.Data is null)
            {
                return new ApiResponse<List<ModuleModel>>(data: null, modules.Status, modules.Message?.Message ?? string.Empty);
            }

            if (modules.Data.Any(m => m.Type == ModuleType.Basic))
            {
                var response = ModuleModel.Convert(modules.Data);

                return new ApiResponse<List<ModuleModel>>(response);
            }

            var basicModule = await moduleService.GetModuleByTypeAsync(ModuleType.Basic, cancellationToken);

            if (basicModule.Data is null)
            {
                return new ApiResponse<List<ModuleModel>>([]);
            }

            modules.Data.Add(basicModule.Data);

            var finalResponse = ModuleModel.Convert(modules.Data);

            return new ApiResponse<List<ModuleModel>>(finalResponse);
        }
        catch
        {
            return new ApiResponse<List<ModuleModel>>(data: null, HttpStatusCode.InternalServerError, "An error occurred while populating modules");
        }
    }
}
