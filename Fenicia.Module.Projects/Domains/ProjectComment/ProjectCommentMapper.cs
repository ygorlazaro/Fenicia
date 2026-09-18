using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.DTOs.Project.ProjectComment;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Projects.Domains.ProjectComment;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ProjectCommentMapper
{
    [MapProperty("User.Name", nameof(GetAllProjectCommentResponse.UserName))]
    public partial GetAllProjectCommentResponse MapToGetAllProjectCommentResponse(ProjectCommentModel comment);

    [MapProperty("User.Name", nameof(GetProjectCommentByIdResponse.UserName))]
    public partial GetProjectCommentByIdResponse MapToGetProjectCommentByIdResponse(ProjectCommentModel comment);

    [MapProperty("User.Name", nameof(AddProjectCommentResponse.UserName))]
    public partial AddProjectCommentResponse MapToAddProjectCommentResponse(ProjectCommentModel comment);

    [MapProperty("User.Name", nameof(UpdateProjectCommentResponse.UserName))]
    public partial UpdateProjectCommentResponse MapToUpdateProjectCommentResponse(ProjectCommentModel comment);
}
