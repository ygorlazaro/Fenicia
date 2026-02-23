using Fenicia.Common.Data.Requests.Project;

namespace Fenicia.Common.Data.Models.Project;

public class CommentModel(CommentRequest request) : BaseModel
{
    public Guid TaskId { get; set; } = request.TaskId;

    public Guid UserId { get; set; } = request.UserId;

    public string Content { get; set; } = request.Content;

    public virtual TaskModel Task { get; set; } = null!;

    public virtual UserModel User { get; set; } = null!;
}