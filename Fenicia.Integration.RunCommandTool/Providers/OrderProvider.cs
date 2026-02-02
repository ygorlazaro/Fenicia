using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Integration.RunCommandTool.Providers;

public class OrderProvider(string uri, string accessToken) : BaseProvider(uri, accessToken)
{
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
