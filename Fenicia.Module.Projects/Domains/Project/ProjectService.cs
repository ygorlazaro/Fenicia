using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Requests.Project;
using Fenicia.Common.Data.Responses.Project;

namespace Fenicia.Module.Projects.Domains.Project;

public class ProjectService(IProjectRepository repository) : IProjectService
{
    public async Task<ProjectResponse?> AddAsync(ProjectRequest request, Guid userId, CancellationToken ct)
    {
        var project = new ProjectModel(request)
        {
            Owner = userId
        };

        repository.Add(project);

        await repository.SaveChangesAsync(ct);

        return new ProjectResponse(project);
    }
}