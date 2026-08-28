using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Basic.Domains.Employee;

public class PositionRepository(DefaultContext context) : Repository<PositionModel>(context)
{
}
