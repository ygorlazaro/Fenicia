using Fenicia.Common.Data.Requests.Project;
using Fenicia.Common.Data.Responses.Project;

namespace Fenicia.Module.Projects.Domains.Project;

public interface IProjectService
{
    Task<ProjectResponse?> AddAsync(ProjectRequest request, Guid userId, CancellationToken ct);
}