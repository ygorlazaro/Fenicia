using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.ProjectComment.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectComment;

public class ProjectCommentRepository(DbContext context) : Repository<ProjectCommentModel>(context), IProjectCommentRepository;