using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models;

[Table("comments", Schema = "project")]
public class ProjectComment : BaseCompanyModel
{
    public Guid TaskId { get; set; } = Guid.Empty;

    public Guid UserId { get; set; } = Guid.Empty;

    public string Content { get; set; } = string.Empty;

    public virtual ProjectTaskModel TaskModel { get; set; } = null!;

    public virtual AuthUser User { get; set; } = null!;

    public Guid AuthorId { get; set; }
}
