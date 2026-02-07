using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.State;

public class StateRepository(BasicContext context) : BaseRepository<StateModel>(context), IStateRepository
{
}
