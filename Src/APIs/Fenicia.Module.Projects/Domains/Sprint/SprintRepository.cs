using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.Sprint.Interfaces;

namespace Fenicia.Module.Projects.Domains.Sprint;

public class SprintRepository(DefaultContext context) : Repository<SprintModel>(context), ISprintRepository
{
}
