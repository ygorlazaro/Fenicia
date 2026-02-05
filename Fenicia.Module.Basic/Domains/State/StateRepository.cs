using Fenicia.Common.Database.Abstracts;
using Fenicia.Common.Database.Contexts;

using Fenicia.Common.Database.Models.Basic;

namespace Fenicia.Module.Basic.Domains.State;

public class StateRepository(BasicContext context) : BaseRepository<StateModel>(context), IStateRepository
{
}
