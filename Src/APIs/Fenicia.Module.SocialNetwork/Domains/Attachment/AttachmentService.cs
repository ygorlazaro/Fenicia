using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Module.SocialNetwork.Domains.Attachment.DTOs;

namespace Fenicia.Module.SocialNetwork.Domains.Attachment;

public class AttachmentService(AttachmentRepository repository)
{
    public async Task<AddAttachmentResponse> AddAsync(AddAttachmentCommand command, Guid companyId, Guid userId, CancellationToken cancellationToken = default)
    {
        var model = new AttachmentModel
        {
            Id = command.Id,
            Url = command.Url,
            FileType = command.FileType,
            FileSize = command.FileSize,
            CommentId = command.CommentId,
            UploadDate = DateTime.UtcNow,
            CompanyId = companyId
        };

        var created = await repository.InsertAsync(model, cancellationToken);
        return new AddAttachmentResponse(created.Id, created.Url, created.FileType, created.FileSize, created.CommentId, created.CompanyId, created.UploadDate);
    }

    public async Task DeleteAsync(DeleteAttachmentCommand command, Guid userId, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(command.Id, cancellationToken);
    }

    public async Task<List<GetAttachmentResponse>> GetByCommentAsync(GetAttachmentsByCommentQuery query, Guid commentId, CancellationToken cancellationToken = default)
    {
        var attachments = await repository.GetByCommentAsync(query.Page, query.PerPage, commentId, cancellationToken);
        return [.. attachments.Select(a => new GetAttachmentResponse(a.Id, a.Url, a.FileType, a.FileSize, a.CommentId, a.UploadDate))];
    }
}
