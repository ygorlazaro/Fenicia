using Fenicia.Module.Projects.Domains.ProjectAttachment.DTOs;

namespace Fenicia.Module.Projects.Domains.ProjectAttachment.Interfaces;

public interface IProjectAttachmentService
{
    Task<List<GetAllProjectAttachmentResponse>> GetAllAsync(
        GetAllProjectAttachmentQuery query,
        CancellationToken cancellationToken = default);

    Task<GetProjectAttachmentByIdResponse?> GetByIdAsync(
        GetProjectAttachmentByIdQuery query,
        CancellationToken cancellationToken = default);

    Task<AddProjectAttachmentResponse> AddAsync(
        AddProjectAttachmentCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<UpdateProjectAttachmentResponse?> UpdateAsync(
        UpdateProjectAttachmentCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(DeleteProjectAttachmentCommand command, CancellationToken cancellationToken = default);
}