using Fenicia.Common;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Module.SocialNetwork.Domains.Comment.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Comment;

public class CommentService(CommentRepository repository)
{
    public async Task<List<GetAllCommentResponse>> GetAllByFeedAsync(GetAllCommentByFeedQuery query, Guid feedId, CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query().Where(c => c.FeedId == feedId && c.ParentCommentId == null).OrderBy(c => c.CommentDate);
        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);
        var comments = await filteredQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(cancellationToken);
        return [.. comments.Select(c => new GetAllCommentResponse(c.Id, c.UserId, c.FeedId, c.ParentCommentId, c.Text, c.CommentDate, c.UpdatedDate))];
    }

    public async Task<GetCommentByIdResponse?> GetByIdAsync(GetCommentByIdQuery query, CancellationToken cancellationToken = default)
    {
        var comment = await repository.GetByIdAsync(query.Id, cancellationToken);

        return comment switch
        {
            null => null,
            _ => new GetCommentByIdResponse(comment.Id, comment.UserId, comment.FeedId, comment.ParentCommentId, comment.Text, comment.CommentDate, comment.UpdatedDate)
        };
    }

    public async Task<AddCommentResponse> AddAsync(AddCommentCommand command, Guid companyId, Guid userId, CancellationToken cancellationToken = default)
    {
        var model = new CommentModel
        {
            Id = command.Id,
            UserId = userId,
            FeedId = command.FeedId,
            ParentCommentId = command.ParentCommentId,
            Text = command.Text,
            CommentDate = DateTime.UtcNow,
            CompanyId = companyId
        };

        var created = await repository.InsertAsync(model, cancellationToken);
        return new AddCommentResponse(created.Id, created.UserId, created.FeedId, created.ParentCommentId, created.Text, created.CommentDate, created.CompanyId);
    }

    public async Task<UpdateCommentResponse?> UpdateAsync(UpdateCommentCommand command, Guid userId, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (existing is null || existing.UserId != userId)
        {
            return null;
        }

        var model = new CommentModel
        {
            Id = command.Id,
            UserId = existing.UserId,
            FeedId = existing.FeedId,
            ParentCommentId = existing.ParentCommentId,
            Text = command.Text,
            CommentDate = existing.CommentDate,
            UpdatedDate = DateTime.UtcNow,
            CompanyId = existing.CompanyId
        };

        var updated = await repository.UpdateAsync(command.Id, model, cancellationToken);
        return updated is null ? null : new UpdateCommentResponse(updated.Id, updated.UserId, updated.FeedId, updated.ParentCommentId, updated.Text, updated.CommentDate, updated.UpdatedDate, updated.CompanyId);
    }

    public async Task DeleteAsync(DeleteCommentCommand command, Guid userId, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (existing is null || existing.UserId != userId)
        {
            return;
        }

        await repository.DeleteAsync(command.Id, cancellationToken);
    }

    public async Task<List<GetRepliesResponse>> GetRepliesAsync(GetRepliesQuery query, CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query().Where(c => c.ParentCommentId == query.ParentCommentId).OrderBy(c => c.CommentDate);
        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);
        var replies = await filteredQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(cancellationToken);
        return [.. replies.Select(r => new GetRepliesResponse(r.Id, r.UserId, r.FeedId, r.ParentCommentId, r.Text, r.CommentDate, r.UpdatedDate))];
    }
}
