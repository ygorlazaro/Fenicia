using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Comment;

public class GetRepliesQuery()
{
    public GetRepliesQuery(int page = 1, int perPage = 10, Guid parentCommentId = default, string? query = null, string? sort = null)
        : this()
    {
        Page = page;
        PerPage = perPage;
        ParentCommentId = parentCommentId;
        Query = query;
        Sort = sort;
    }

    public int Page { get; set; } = 1;

    public int PerPage { get; set; } = 10;

    [Required]
    public Guid ParentCommentId { get; set; }

    public string? Query { get; set; }

    public string? Sort { get; set; }
}
