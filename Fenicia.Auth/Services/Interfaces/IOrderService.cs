using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Requests;
using Fenicia.Auth.Responses;

namespace Fenicia.Auth.Services.Interfaces;

public interface IOrderService
{
    Task<OrderResponse> CreateNewOrderAsync(Guid userId, Guid companyId, NewOrderRequest request);
}