namespace Fenicia.Common.DTOs.SocialNetwork.Attachment;

public class GetAttachmentsByCommentQuery()
{
    public GetAttachmentsByCommentQuery(int page = 1, int perPage = 10, string? query = null, string? sort = null)
        : this()
    {
        Page = page;
        PerPage = perPage;
        Query = query;
        Sort = sort;
    }

    public int Page { get; set; } = 1;

    public int PerPage { get; set; } = 10;

    public string? Query { get; set; }

    public string? Sort { get; set; }
}
