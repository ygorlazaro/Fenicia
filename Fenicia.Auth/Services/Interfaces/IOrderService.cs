using Fenicia.Auth.Requests;
using Fenicia.Auth.Responses;
using Fenicia.Common;

namespace Fenicia.Auth.Services.Interfaces;

public interface IOrderService
{
    Task<ServiceResponse<OrderResponse>> CreateNewOrderAsync(Guid userId, Guid companyId, OrderRequest request);
}