using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.DTOs.SocialNetwork.Attachment;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Attachment;

public class AttachmentService(AttachmentRepository repository, AttachmentMapper mapper)
{
    public async Task<AddAttachmentResponse> AddAsync(
        AddAttachmentCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
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
        return mapper.MapToAddAttachmentResponse(created);
    }

    public async Task DeleteAsync(DeleteAttachmentCommand command, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(command.Id, cancellationToken);
    }

    public async Task<List<GetAttachmentResponse>> GetByCommentAsync(
        GetAttachmentsByCommentQuery query,
        Guid commentId,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query().Where(a => a.CommentId == commentId);
        var attachments = await baseQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage)
            .ToListAsync(cancellationToken);
        return [.. attachments.Select(mapper.MapToGetAttachmentResponse)];
    }
}
