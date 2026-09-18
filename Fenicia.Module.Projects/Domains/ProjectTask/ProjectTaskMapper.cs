using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.DTOs.Project.ProjectTask;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Projects.Domains.ProjectTask;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ProjectTaskMapper
{
    [MapProperty("SprintModel.Name", nameof(AddProjectTaskResponse.SprintName))]
    public partial AddProjectTaskResponse MapToAddProjectTaskResponse(ProjectTaskModel task);

    [MapProperty("SprintModel.Name", nameof(UpdateProjectTaskResponse.SprintName))]
    public partial UpdateProjectTaskResponse MapToUpdateProjectTaskResponse(ProjectTaskModel task);

    [MapProperty("User.Name", nameof(ProjectTaskAssigneeResponse.UserName))]
    [MapProperty("User.Email", nameof(ProjectTaskAssigneeResponse.UserEmail))]
    public partial ProjectTaskAssigneeResponse MapToProjectTaskAssigneeResponse(TaskAssigneeModel assignee);
}
