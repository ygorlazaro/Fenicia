using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.Project;

[Table("comments", Schema = "project")]
public sealed class ProjectCommentModel : BaseCompanyModel
{
    public Guid TaskId { get; init; } = Guid.Empty;

    public Guid UserId { get; init; } = Guid.Empty;

    [MaxLength(4096)]
    public string Content { get; set; } = string.Empty;

    [ForeignKey(nameof(TaskId))]
    public ProjectTaskModel TaskModel { get; init; } = null!;

    [ForeignKey(nameof(UserId))]
    public UserModel User { get; init; } = null!;

    public Guid AuthorId { get; init; }
}
