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
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    [Column("password")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [StringLength(48)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    public List<UserRoleModel> UsersRoles { get; init; } = [];

    public List<OrderModel> Orders { get; init; } = [];

    public List<ProjectTaskModel> Tasks { get; init; } = [];

    [MaxLength(48)]
    public string? ImageUrl { get; init; }

    public List<TaskAssigneeModel> TaskAssignees { get; init; } = [];

    public List<ConfigurationModel> Configurations { get; init; } = [];
}