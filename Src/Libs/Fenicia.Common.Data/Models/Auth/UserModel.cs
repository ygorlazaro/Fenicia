using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Project;

namespace Fenicia.Common.Data.Models.Auth;

[Table("users", Schema = "auth")]
public class UserModel : BaseModel
{
    [Required]
    [EmailAddress]
    [StringLength(48)]
    [Column("email")]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(200)]
    [Column("password")]
    public string Password { get; set; } = null!;

    [Required]
    [StringLength(48)]
    [Column("name")]
    public string Name { get; set; } = null!;

    public List<UserRoleModel> UsersRoles { get; set; } = [];

    public List<OrderModel> Orders { get; set; } = [];

    public List<ProjectTaskModel> Tasks { get; set; } = [];

    [MaxLength(48)]
    public string? ImageUrl { get; set; }

    public List<TaskAssigneeModel> TaskAssignees { get; set; } = [];

    public List<ConfigurationModel> Configurations { get; set; } = [];
}