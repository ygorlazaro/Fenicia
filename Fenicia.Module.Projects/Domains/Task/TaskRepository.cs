using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Project;

namespace Fenicia.Module.Projects.Domains.Task;

public class TaskRepository(ProjectContext context) : BaseRepository<TaskModel>(context), ITaskRepository
{
    
}