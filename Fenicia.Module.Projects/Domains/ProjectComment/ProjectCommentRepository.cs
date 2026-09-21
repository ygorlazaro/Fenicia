using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.ProjectComment.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectComment;

public class ProjectCommentRepository(DbContext context) : Repository<ProjectCommentModel>(context), IProjectCommentRepository
{
    public new IQueryable<ProjectCommentModel> Query()
    {
        return base.Query();
    }

    public new Task<ProjectCommentModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.GetByIdAsync(id, cancellationToken);
    }

    public new Task<ProjectCommentModel> InsertAsync(ProjectCommentModel model, CancellationToken cancellationToken = default)
    {
        return base.InsertAsync(model, cancellationToken);
    }

    public new Task<ProjectCommentModel?> UpdateAsync(Guid id, ProjectCommentModel model, CancellationToken cancellationToken = default)
    {
        return base.UpdateAsync(id, model, cancellationToken);
    }

    public new Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.DeleteAsync(id, cancellationToken);
    }

    public new Task<IEnumerable<ProjectCommentModel>> GetAllAsync(int page, int perPage, CancellationToken cancellationToken = default)
    {
        return base.GetAllAsync(page, perPage, cancellationToken);
    }
}