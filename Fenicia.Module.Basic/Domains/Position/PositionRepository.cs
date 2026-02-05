using Fenicia.Common.Database.Abstracts;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Position;

public class PositionRepository(BasicContext context) : BaseRepository<PositionModel>(context), IPositionRepository
{
}
