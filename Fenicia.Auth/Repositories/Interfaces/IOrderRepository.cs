using Fenicia.Auth.Contexts.Models;

namespace Fenicia.Auth.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<OrderModel> SaveOrderAsync(OrderModel order);
}