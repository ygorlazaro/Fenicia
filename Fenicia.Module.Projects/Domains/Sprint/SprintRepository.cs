using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.Sprint.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.Sprint;

public class SprintRepository(DbContext context) : Repository<SprintModel>(context), ISprintRepository
{
}
