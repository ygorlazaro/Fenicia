using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Auth.Domains.Order.Interfaces;

/// <summary>
/// Interface for the order repository, providing methods to manage order data.
/// </summary>
public interface IOrderRepository : IRepository<OrderModel>;
