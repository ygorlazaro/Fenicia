using Fenicia.Auth.Domains.Order.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Order;

public class OrderRepository(DbContext context) : Repository<OrderModel>(context), IOrderRepository;