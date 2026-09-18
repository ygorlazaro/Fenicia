using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.DTOs.Project.ProjectStatus;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Projects.Domains.ProjectStatus;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ProjectStatusMapper
{
    public partial GetAllProjectStatusResponse MapToGetAllProjectStatusResponse(ProjectStatusModel status);

    public partial GetProjectStatusByIdResponse MapToGetProjectStatusByIdResponse(ProjectStatusModel status);

    public partial AddProjectStatusResponse MapToAddProjectStatusResponse(ProjectStatusModel status);

    public partial UpdateProjectStatusResponse MapToUpdateProjectStatusResponse(ProjectStatusModel status);
}
