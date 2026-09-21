using Fenicia.Common.Data.Models.Project;

namespace Fenicia.Module.Projects.Domains.ProjectComment.Interfaces;

public interface IProjectCommentRepository
{
    IQueryable<ProjectCommentModel> Query();

    Task<ProjectCommentModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ProjectCommentModel> InsertAsync(ProjectCommentModel model, CancellationToken cancellationToken = default);

    Task<ProjectCommentModel?> UpdateAsync(Guid id, ProjectCommentModel model, CancellationToken cancellationToken = default);

    Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<ProjectCommentModel>> GetAllAsync(int page, int perPage, CancellationToken cancellationToken = default);
}