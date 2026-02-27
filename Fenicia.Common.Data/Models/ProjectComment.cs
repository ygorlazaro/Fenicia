using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models;

[Table("comments", Schema = "project")]
public class ProjectComment : BaseModel
{
    public Guid TaskId { get; set; } = Guid.Empty;

    public Guid UserId { get; set; } = Guid.Empty;

    public string Content { get; set; } = string.Empty;

    public virtual ProjectTask Task { get; set; } = null!;

    public virtual AuthUser User { get; set; } = null!;
}
