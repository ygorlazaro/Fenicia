using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee;

public class ProjectTaskAssigneeRepository(DefaultContext context) : Repository<TaskAssigneeModel>(context)
{
}
