using Fenicia.Common.DTOs.SocialNetwork.Attachment;

namespace Fenicia.Module.SocialNetwork.Domains.Attachment.Interfaces;

public interface IAttachmentService
{
    Task<AddAttachmentResponse> AddAsync(AddAttachmentCommand command, Guid companyId, CancellationToken cancellationToken = default);
    Task DeleteAsync(DeleteAttachmentCommand command, CancellationToken cancellationToken = default);
    Task<List<GetAttachmentResponse>> GetByCommentAsync(GetAttachmentsByCommentQuery query, Guid commentId, CancellationToken cancellationToken = default);
}