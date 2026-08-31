using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Module.SocialNetwork.Domains.Attachment.DTOs;

namespace Fenicia.Module.SocialNetwork.Domains.Attachment;

public class AttachmentService(AttachmentRepository repository)
{
    public async Task<AddAttachmentResponse> AddAsync(AddAttachmentCommand command, Guid companyId, Guid userId, CancellationToken ct)
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

        var created = await repository.InsertAsync(model, ct);
        return new AddAttachmentResponse(created.Id, created.Url, created.FileType, created.FileSize, created.CommentId, created.CompanyId, created.UploadDate);
    }

    public async Task DeleteAsync(DeleteAttachmentCommand command, Guid userId, CancellationToken ct)
    {
        await repository.DeleteAsync(command.Id, ct);
    }

    public async Task<List<GetAttachmentResponse>> GetByCommentAsync(GetAttachmentsByCommentQuery query, Guid commentId, CancellationToken ct)
    {
        var attachments = await repository.GetByCommentAsync(query.Page, query.PerPage, commentId, ct);
        return [.. attachments.Select(a => new GetAttachmentResponse(a.Id, a.Url, a.FileType, a.FileSize, a.CommentId, a.UploadDate))];
    }
}
