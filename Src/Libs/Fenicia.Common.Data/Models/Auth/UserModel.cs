using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Data.Models.SocialNetworkModels;

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

    [InverseProperty(nameof(FollowerModel.Follower))]
    public List<FollowerModel> Followers { get; set; } = [];

    [InverseProperty(nameof(FollowerModel.UserModel))]
    public List<FollowerModel> Following { get; set; } = [];

    public List<TaskAssigneeModel> TaskAssignees { get; set; } = [];

    public List<ConfigurationModel> Configurations { get; set; } = [];
}