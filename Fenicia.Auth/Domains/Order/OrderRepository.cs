using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Auth.Domains.Order;

public class OrderRepository(DefaultContext context) : Repository<OrderModel>(context);