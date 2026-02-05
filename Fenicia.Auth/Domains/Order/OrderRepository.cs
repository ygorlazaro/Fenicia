using Fenicia.Common.Database.Abstracts;
using Fenicia.Common.Database.Contexts;

using Fenicia.Common.Database.Models.Auth;

namespace Fenicia.Auth.Domains.Order;

public class OrderRepository(AuthContext context) : BaseRepository<OrderModel>(context), IOrderRepository
{
}
