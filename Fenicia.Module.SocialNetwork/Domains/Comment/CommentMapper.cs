using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.DTOs.SocialNetwork.Comment;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.SocialNetwork.Domains.Comment;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CommentMapper
{
    public partial GetCommentByIdResponse MapToGetCommentByIdResponse(CommentModel comment);

    public partial AddCommentResponse MapToAddCommentResponse(CommentModel comment);

    public partial UpdateCommentResponse MapToUpdateCommentResponse(CommentModel comment);
}
