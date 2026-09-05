using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Module.SocialNetwork.Domains.Comment.DTOs;
using Fenicia.Module.SocialNetwork.Domains.Feed;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Comment;

public class CommentService(CommentRepository repository, FeedRepository feedRepository)
{
    public async Task<List<GetAllCommentResponse>> GetAllByFeedAsync(
        GetAllCommentByFeedQuery query,
        Guid feedId,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query().Where(c => c.FeedId == feedId && c.ParentCommentId == null)
            .OrderBy(c => c.CommentDate);
        var filteredQuery = baseQuery;
        var comments = await filteredQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage)
            .ToListAsync(cancellationToken);
        return
        [
            .. comments.Select(c => new GetAllCommentResponse(
                c.Id,
                c.ProfileId,
                c.FeedId,
                c.ParentCommentId,
                c.Text,
                c.CommentDate,
                c.UpdatedDate))
        ];
    }

    public async Task<GetCommentByIdResponse?> GetByIdAsync(
        GetCommentByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var comment = await repository.GetByIdAsync(query.Id, cancellationToken);

        return comment switch
        {
            null => null,
            _ => new GetCommentByIdResponse(
                comment.Id,
                comment.ProfileId,
                comment.FeedId,
                comment.ParentCommentId,
                comment.Text,
                comment.CommentDate,
                comment.UpdatedDate)
        };
    }

    public async Task<AddCommentResponse> AddAsync(
        AddCommentCommand command,
        Guid companyId,
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        var model = new CommentModel
        {
            Id = command.Id,
            ProfileId = profileId,
            FeedId = command.FeedId,
            ParentCommentId = command.ParentCommentId,
            Text = command.Text,
            CommentDate = DateTime.UtcNow,
            CompanyId = companyId
        };

        var created = await repository.InsertAsync(model, cancellationToken);
        await IncrementFeedTotalCommentsAsync(command.FeedId, cancellationToken);
        return new AddCommentResponse(
            created.Id,
            created.ProfileId,
            created.FeedId,
            created.ParentCommentId,
            created.Text,
            created.CommentDate,
            created.CompanyId);
    }

    public async Task<UpdateCommentResponse?> UpdateAsync(
        UpdateCommentCommand command,
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (existing is null || existing.ProfileId != profileId)
        {
            return null;
        }

        var model = new CommentModel
        {
            Id = command.Id,
            ProfileId = existing.ProfileId,
            FeedId = existing.FeedId,
            ParentCommentId = existing.ParentCommentId,
            Text = command.Text,
            CommentDate = existing.CommentDate,
            UpdatedDate = DateTime.UtcNow,
            CompanyId = existing.CompanyId
        };

        var updated = await repository.UpdateAsync(command.Id, model, cancellationToken);
        return updated is null
            ? null
            : new UpdateCommentResponse(
                updated.Id,
                updated.ProfileId,
                updated.FeedId,
                updated.ParentCommentId,
                updated.Text,
                updated.CommentDate,
                updated.UpdatedDate,
                updated.CompanyId);
    }

    public async Task DeleteAsync(
        DeleteCommentCommand command,
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (existing is null || existing.ProfileId != profileId)
        {
            return;
        }

        await repository.DeleteAsync(command.Id, cancellationToken);
        await DecrementFeedTotalCommentsAsync(existing.FeedId, cancellationToken);
    }

    public async Task<List<GetRepliesResponse>> GetRepliesAsync(
        GetRepliesQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query().Where(c => c.ParentCommentId == query.ParentCommentId)
            .OrderBy(c => c.CommentDate);
        var filteredQuery = baseQuery;
        var replies = await filteredQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage)
            .ToListAsync(cancellationToken);
        return
        [
            .. replies.Select(r => new GetRepliesResponse(
                r.Id,
                r.ProfileId,
                r.FeedId,
                r.ParentCommentId,
                r.Text,
                r.CommentDate,
                r.UpdatedDate))
        ];
    }

    private async Task IncrementFeedTotalCommentsAsync(Guid feedId, CancellationToken cancellationToken)
    {
        var feed = await feedRepository.GetByIdAsync(feedId, cancellationToken);
        if (feed is null)
        {
            return;
        }

        feed.TotalComments++;
        await feedRepository.UpdateAsync(feedId, feed, cancellationToken);
    }

    private async Task DecrementFeedTotalCommentsAsync(Guid feedId, CancellationToken cancellationToken)
    {
        var feed = await feedRepository.GetByIdAsync(feedId, cancellationToken);
        if (feed is null)
        {
            return;
        }

        feed.TotalComments = Math.Max(0, feed.TotalComments - 1);
        await feedRepository.UpdateAsync(feedId, feed, cancellationToken);
    }
}
