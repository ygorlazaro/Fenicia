using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.ProjectSubtask.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectSubtask;

public class ProjectSubtaskRepository(DbContext context)
    : Repository<ProjectStatusModel>(context), IProjectSubtaskRepository;