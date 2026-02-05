using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;

using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.Order;

public class OrderRepository(AuthContext context) : BaseRepository<OrderModel>(context), IOrderRepository
{
}
