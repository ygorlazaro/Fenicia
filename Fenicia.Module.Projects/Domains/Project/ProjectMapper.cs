using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.DTOs.Project.Project;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Projects.Domains.Project;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ProjectMapper
{
    public partial GetAllProjectResponse MapToGetAllProjectResponse(ProjectModel project);

    public partial GetProjectByIdResponse MapToGetProjectByIdResponse(ProjectModel project);

    public partial AddProjectResponse MapToAddProjectResponse(ProjectModel project);

    public partial UpdateProjectResponse MapToUpdateProjectResponse(ProjectModel project);
}
