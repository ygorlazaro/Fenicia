using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Projects.Domains.ProjectComment;

public class ProjectCommentRepository(DefaultContext context) : Repository<ProjectCommentModel>(context);