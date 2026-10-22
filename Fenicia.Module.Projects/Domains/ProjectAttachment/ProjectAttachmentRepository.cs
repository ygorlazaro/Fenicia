using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.ProjectAttachment.Interfaces;
using Microsoft.EntityFrameworkCore;
using Fenicia.Common;

/// <summary>
/// Fenicia.Module.Projects/Domains/ProjectAttachment/ProjectAttachmentRepository.cs implementation.
/// </summary>

namespace Fenicia.Module.Projects.Domains.ProjectAttachment;

public class ProjectAttachmentRepository(DbContext context) : Repository<AttachmentModel>(context), IProjectAttachmentRepository
{
    public override IQueryable<AttachmentModel> Query()
    {
        return base.Query();
    }

    public override Task<AttachmentModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.GetByIdAsync(id, cancellationToken);
    }

    public override Task<AttachmentModel> InsertAsync(AttachmentModel model, CancellationToken cancellationToken = default)
    {
        return base.InsertAsync(model, cancellationToken);
    }

    public override Task<AttachmentModel?> UpdateAsync(Guid id, AttachmentModel model, CancellationToken cancellationToken = default)
    {
        return base.UpdateAsync(id, model, cancellationToken);
    }

    public override Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.DeleteAsync(id, cancellationToken);
    }

    public override Task<IEnumerable<AttachmentModel>> GetAllAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        return base.GetAllAsync(query, cancellationToken);
    }
}