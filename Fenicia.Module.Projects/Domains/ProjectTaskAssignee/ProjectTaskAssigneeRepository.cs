using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee;

public class ProjectTaskAssigneeRepository(DbContext context) : Repository<TaskAssigneeModel>(context),
    IProjectTaskAssigneeRepository;