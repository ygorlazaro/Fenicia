using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Order;

public class OrderRepository(BasicContext context) : BaseRepository<OrderModel>(context), IOrderRepository
{
}