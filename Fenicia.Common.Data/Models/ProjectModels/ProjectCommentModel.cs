using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.ProjectModels;

[Table("comments", Schema = "project")]
public class ProjectCommentModel : BaseCompanyModel
{
    public Guid TaskId { get; set; } = Guid.Empty;

    public Guid UserId { get; set; } = Guid.Empty;

    [MaxLength(4096)]
    public string Content { get; set; } = string.Empty;

    public virtual ProjectTaskModel TaskModel { get; set; } = null!;

    public virtual UserModel User { get; set; } = null!;

    public Guid AuthorId { get; set; }
}
