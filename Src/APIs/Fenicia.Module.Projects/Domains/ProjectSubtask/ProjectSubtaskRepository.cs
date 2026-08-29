using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Projects.Domains.ProjectSubtask;

public class ProjectSubtaskRepository(DefaultContext context) : Repository<ProjectSubtaskModel>(context)
{
}
