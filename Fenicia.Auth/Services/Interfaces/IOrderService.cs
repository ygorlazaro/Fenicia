using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Requests;

namespace Fenicia.Auth.Services.Interfaces;

public interface IOrderService
{
    Task<OrderModel?> CreateNewOrderAsync(Guid userId, Guid companyId, NewOrderRequest request);
}