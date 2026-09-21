using Fenicia.Common.DTOs.SocialNetwork.Comment;

namespace Fenicia.Module.SocialNetwork.Domains.Comment.Interfaces;

public interface ICommentService
{
    Task<List<GetAllCommentResponse>> GetAllByFeedAsync(GetAllCommentByFeedQuery query, Guid feedId, Guid profileId, CancellationToken cancellationToken = default);
    Task<GetCommentByIdResponse?> GetByIdAsync(GetCommentByIdQuery query, CancellationToken cancellationToken = default);
    Task<AddCommentResponse> AddAsync(AddCommentCommand command, Guid companyId, Guid profileId, CancellationToken cancellationToken = default);
    Task<UpdateCommentResponse?> UpdateAsync(UpdateCommentCommand command, Guid profileId, CancellationToken cancellationToken = default);
    Task DeleteAsync(DeleteCommentCommand command, Guid profileId, CancellationToken cancellationToken = default);
    Task<List<GetRepliesResponse>> GetRepliesAsync(GetRepliesQuery query, Guid profileId, CancellationToken cancellationToken = default);
}