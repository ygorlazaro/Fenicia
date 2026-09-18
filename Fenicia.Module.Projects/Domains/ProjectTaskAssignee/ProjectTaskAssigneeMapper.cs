using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.DTOs.Project.ProjectTaskAssignee;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ProjectTaskAssigneeMapper
{
    public partial GetAllProjectTaskAssigneeResponse MapToGetAllProjectTaskAssigneeResponse(TaskAssigneeModel assignee);

    public partial GetProjectTaskAssigneeByIdResponse MapToGetProjectTaskAssigneeByIdResponse(TaskAssigneeModel assignee);

    public partial AddProjectTaskAssigneeResponse MapToAddProjectTaskAssigneeResponse(TaskAssigneeModel assignee);

    public partial UpdateProjectTaskAssigneeResponse MapToUpdateProjectTaskAssigneeResponse(TaskAssigneeModel assignee);
}
