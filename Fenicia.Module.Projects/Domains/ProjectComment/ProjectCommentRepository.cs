using Fenicia.Common;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.ProjectComment.Interfaces;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Fenicia.Module.Projects/Domains/ProjectComment/ProjectCommentRepository.cs implementation.
/// </summary>

namespace Fenicia.Module.Projects.Domains.ProjectComment;

public class ProjectCommentRepository(DbContext context) : Repository<ProjectCommentModel>(context), IProjectCommentRepository
{
    public override IQueryable<ProjectCommentModel> Query()
    {
        return base.Query();
    }

    public override Task<ProjectCommentModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.GetByIdAsync(id, cancellationToken);
    }

    public override Task<ProjectCommentModel> InsertAsync(ProjectCommentModel model, CancellationToken cancellationToken = default)
    {
        return base.InsertAsync(model, cancellationToken);
    }

    public override Task<ProjectCommentModel?> UpdateAsync(Guid id, ProjectCommentModel model, CancellationToken cancellationToken = default)
    {
        return base.UpdateAsync(id, model, cancellationToken);
    }

    public override Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.DeleteAsync(id, cancellationToken);
    }

    public override Task<IEnumerable<ProjectCommentModel>> GetAllAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        return base.GetAllAsync(query, cancellationToken);
    }
}