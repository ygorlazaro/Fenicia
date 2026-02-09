namespace Fenicia.Common.Data.Requests.Project;

public class CommentRequest
{
    public Guid TaskId { get; set; }

    public string Content { get; set; } = string.Empty;

    public Guid UserId { get; set; }
}