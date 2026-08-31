using Fenicia.Common;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Module.SocialNetwork.Domains.Attachment.DTOs;
using Microsoft.EntityFrameworkCore;

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
        var baseQuery = repository.Query().Where(a => a.CommentId == commentId);
        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);
        var attachments = await filteredQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(cancellationToken);
        return [.. attachments.Select(a => new GetAttachmentResponse(a.Id, a.Url, a.FileType, a.FileSize, a.CommentId, a.UploadDate))];
    }
}
