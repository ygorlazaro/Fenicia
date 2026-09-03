using Fenicia.Module.Projects.Domains.ProjectComment.DTOs;

namespace Fenicia.Module.Projects.Domains.ProjectComment.Interfaces;

public interface IProjectCommentService
{
    Task<List<GetAllProjectCommentResponse>> GetAllAsync(
        GetAllProjectCommentQuery query,
        CancellationToken cancellationToken = default);

    Task<GetProjectCommentByIdResponse?> GetByIdAsync(
        GetProjectCommentByIdQuery query,
        CancellationToken cancellationToken = default);

    Task<AddProjectCommentResponse> AddAsync(
        AddProjectCommentCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<UpdateProjectCommentResponse?> UpdateAsync(
        UpdateProjectCommentCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(DeleteProjectCommentCommand command, CancellationToken cancellationToken = default);
}