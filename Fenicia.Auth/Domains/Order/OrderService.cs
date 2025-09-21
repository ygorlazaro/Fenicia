namespace Fenicia.Auth.Domains.Order;

using System.Net;

using Common;
using Common.Enums;

using Fenicia.Common.Database.Models.Auth;
using Common.Database.Requests;
using Common.Database.Responses;
using Module;
using Subscription;
using User;

public sealed class OrderService : IOrderService
{
    private readonly ILogger<OrderService> logger;
    private readonly IOrderRepository orderRepository;
    private readonly IModuleService moduleService;
    private readonly ISubscriptionService subscriptionService;
    private readonly IUserService userService;

    public OrderService(ILogger<OrderService> logger, IOrderRepository orderRepository, IModuleService moduleService, ISubscriptionService subscriptionService, IUserService userService)
    {
        this.logger = logger;
        this.orderRepository = orderRepository;
        this.moduleService = moduleService;
        this.subscriptionService = subscriptionService;
        this.userService = userService;
    }

    public async Task<ApiResponse<OrderResponse>> CreateNewOrderAsync(Guid userId, Guid companyId, OrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation("Starting order creation process for user {UserID} in company {CompanyID}", userId, companyId);
            var existingUser = await this.userService.ExistsInCompanyAsync(userId, companyId, cancellationToken);

            if (!existingUser.Data)
            {
                this.logger.LogWarning("User {userID} does not exist in company {companyID}", userId, companyId);

                return new ApiResponse<OrderResponse>(data: null, HttpStatusCode.BadRequest, TextConstants.UserNotInCompanyMessage);
            }

            var modules = await this.PopulateModules(request, cancellationToken);

            if (modules.Data is null)
            {
                return new ApiResponse<OrderResponse>(data: null, modules.Status, modules.Message.Message ?? string.Empty);
            }

            if (modules.Data.Count == 0)
            {
                this.logger.LogWarning("There was an error searching modules");

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

            await this.orderRepository.SaveOrderAsync(order, cancellationToken);
            await this.subscriptionService.CreateCreditsForOrderAsync(order, details, companyId, cancellationToken);

            var response = OrderResponse.Convert(order);

            this.logger.LogInformation("Order created successfully for user {UserID}", userId);
            return new ApiResponse<OrderResponse>(response);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error creating order for user {UserID} in company {CompanyID}", userId, companyId);
            return new ApiResponse<OrderResponse>(data: null, HttpStatusCode.InternalServerError, "An error occurred while creating the order");
        }
    }

    private async Task<ApiResponse<List<ModuleModel>>> PopulateModules(OrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogDebug("Starting to populate modules for order request");
            var uniqueModules = request.Details.Select(d => d.ModuleId).Distinct();
            var modules = await this.moduleService.GetModulesToOrderAsync(uniqueModules, cancellationToken);

            if (modules.Data is null)
            {
                return new ApiResponse<List<ModuleModel>>(data: null, modules.Status, modules.Message.Message ?? string.Empty);
            }

            if (modules.Data.Any(m => m.Type == ModuleType.Basic))
            {
                var response = ModuleModel.Convert(modules.Data);

                return new ApiResponse<List<ModuleModel>>(response);
            }

            var basicModule = await this.moduleService.GetModuleByTypeAsync(ModuleType.Basic, cancellationToken);

            if (basicModule.Data is null)
            {
                return new ApiResponse<List<ModuleModel>>([]);
            }

            modules.Data.Add(basicModule.Data);

            var finalResponse = ModuleModel.Convert(modules.Data);

            this.logger.LogInformation("Modules populated successfully");
            return new ApiResponse<List<ModuleModel>>(finalResponse);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error populating modules for order request");
            return new ApiResponse<List<ModuleModel>>(data: null, HttpStatusCode.InternalServerError, "An error occurred while populating modules");
        }
    }
}
