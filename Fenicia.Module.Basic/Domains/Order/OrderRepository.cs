using Fenicia.Common.Database.Abstracts;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Order;

public class OrderRepository(BasicContext context) : BaseRepository<OrderModel>(context), IOrderRepository
{
}
