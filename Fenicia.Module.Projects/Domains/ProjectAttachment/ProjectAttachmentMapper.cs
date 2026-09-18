using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.DTOs.Project.ProjectAttachment;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Projects.Domains.ProjectAttachment;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ProjectAttachmentMapper
{
    public partial GetAllProjectAttachmentResponse MapToGetAllProjectAttachmentResponse(AttachmentModel attachment);

    public partial GetProjectAttachmentByIdResponse MapToGetProjectAttachmentByIdResponse(AttachmentModel attachment);

    public partial AddProjectAttachmentResponse MapToAddProjectAttachmentResponse(AttachmentModel attachment);

    public partial UpdateProjectAttachmentResponse MapToUpdateProjectAttachmentResponse(AttachmentModel attachment);
}
