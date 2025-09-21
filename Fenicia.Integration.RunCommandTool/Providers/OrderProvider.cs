namespace Fenicia.Integration.RunCommandTool.Providers;

using Common.Database.Requests;
using Common.Database.Responses;

public class OrderProvider : BaseProvider
{
    public OrderProvider(string uri, string accessToken)
        : base(uri, accessToken)
    {
    }

    public async Task CreateOrderAsync(List<ModuleResponse> modules)
    {
        var orderRequest = new OrderRequest
                           {
            Details = modules.Select(module => new OrderDetailRequest
            {
                ModuleId = module.Id
            })
                           };

        await this.PostAsync<OrderResponse, OrderRequest>("order", orderRequest);
    }
}
