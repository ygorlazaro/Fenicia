using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Project;

[Table("users")]
public  class UserModel : BaseModel
{
    [Required]
    [EmailAddress]
    [StringLength(48)]
    [Column("email")]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(48)]
    [Column("name")]
    public string Name { get; set; } = null!;

    public virtual List<CommentModel> Comments { get; set; } = [];

    public virtual List<AttachmentModel> Attachments { get; set; } = [];

    public virtual List<TaskModel> Tasks { get; set; } = [];
}