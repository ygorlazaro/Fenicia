using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Position;

public class PositionRepository(BasicContext context) : BaseRepository<PositionModel>(context), IPositionRepository
{
}