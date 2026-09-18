using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.DTOs.Project.ProjectSubtask;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Projects.Domains.ProjectSubtask;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ProjectSubtaskMapper
{
    public partial GetAllProjectSubtaskResponse MapToGetAllProjectSubtaskResponse(ProjectSubtaskModel subtask);

    public partial GetProjectSubtaskByIdResponse MapToGetProjectSubtaskByIdResponse(ProjectSubtaskModel subtask);

    public partial AddProjectSubtaskResponse MapToAddProjectSubtaskResponse(ProjectSubtaskModel subtask);

    public partial UpdateProjectSubtaskResponse MapToUpdateProjectSubtaskResponse(ProjectSubtaskModel subtask);
}
