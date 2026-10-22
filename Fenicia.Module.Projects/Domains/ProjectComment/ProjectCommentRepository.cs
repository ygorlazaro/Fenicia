using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.ProjectComment.Interfaces;
using Microsoft.EntityFrameworkCore;
using Fenicia.Common;

/// <summary>
/// Fenicia.Module.Projects/Domains/ProjectComment/ProjectCommentRepository.cs implementation.
/// </summary>

namespace Fenicia.Module.Projects.Domains.ProjectComment;

public class ProjectCommentRepository(DbContext context) : Repository<ProjectCommentModel>(context), IProjectCommentRepository
{
    public IQueryable<ProjectCommentModel> Query()
    {
        return base.Query();
    }

    public Task<ProjectCommentModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.GetByIdAsync(id, cancellationToken);
    }

    public Task<ProjectCommentModel> InsertAsync(ProjectCommentModel model, CancellationToken cancellationToken = default)
    {
        return base.InsertAsync(model, cancellationToken);
    }

    public Task<ProjectCommentModel?> UpdateAsync(Guid id, ProjectCommentModel model, CancellationToken cancellationToken = default)
    {
        return base.UpdateAsync(id, model, cancellationToken);
    }

    public Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.DeleteAsync(id, cancellationToken);
    }

    public Task<IEnumerable<ProjectCommentModel>> GetAllAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        return base.GetAllAsync(page, perPage, cancellationToken);
    }
}