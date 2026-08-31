using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Comment.DTOs;

public record GetAllCommentByFeedQuery(
    int Page = 1,
    int PerPage = 10,
    [Required] Guid FeedId = default);
