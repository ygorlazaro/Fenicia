using Fenicia.Common.Data.Models.Project;

namespace Fenicia.Module.Projects.Domains.ProjectAttachment.Interfaces;

public interface IProjectAttachmentRepository
{
    IQueryable<AttachmentModel> Query();

    Task<AttachmentModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<AttachmentModel> InsertAsync(AttachmentModel model, CancellationToken cancellationToken = default);

    Task<AttachmentModel?> UpdateAsync(Guid id, AttachmentModel model, CancellationToken cancellationToken = default);

    Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<AttachmentModel>> GetAllAsync(int page, int perPage, CancellationToken cancellationToken = default);
}