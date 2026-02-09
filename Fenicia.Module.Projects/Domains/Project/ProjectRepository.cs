using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Project;

namespace Fenicia.Module.Projects.Domains.Project;

public class ProjectRepository(ProjectContext context) : BaseRepository<ProjectModel>(context), IProjectRepository
{
    
}