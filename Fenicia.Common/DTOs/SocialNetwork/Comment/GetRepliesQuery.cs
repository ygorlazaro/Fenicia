using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Comment;

public record GetRepliesQuery(
    int Page = 1,
    int PerPage = 10,
    [Required] Guid ParentCommentId = default,
    string? Query = null,
    string? Sort = null);