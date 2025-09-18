namespace Fenicia.Integration.RunCommandTool.Providers;

using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

public class OrderProvider : BaseProvider
{
    public OrderProvider(string baseUrl)
        : base(baseUrl)
    {
    }

    public OrderProvider(string baseUrl, string accessToken)
        : base(baseUrl, accessToken)
    {
    }

    public async Task<OrderResponse> CreateOrderAsync(List<ModuleResponse> modules)
    {
        var orderRequest = new OrderRequest()
        {
            Details = modules.Select(module => new OrderDetailRequest
            {
                ModuleId = module.Id
            })
        };

        return await this.PostAsync<OrderResponse, OrderRequest>("order", orderRequest);
    }
}
