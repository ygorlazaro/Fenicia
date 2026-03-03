using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Fenicia.Common.Data.Models;

[Table("users", Schema = "auth")]
public class AuthUser : BaseModel
{
    [Required]
    [EmailAddress]
    [StringLength(48)]
    [Column("email")]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(200)]
    [JsonIgnore]
    [Column("password")]
    public string Password { get; set; } = null!;

    [Required]
    [StringLength(48)]
    [Column("name")]
    public string Name { get; set; } = null!;

    [JsonIgnore]
    public List<AuthUserRoleModel> UsersRoles { get; set; } = [];

    [JsonIgnore]
    public List<AuthOrder> Orders { get; set; } = [];

    [JsonIgnore]
    public List<ProjectTaskModel> Tasks { get; set; } = [];

    [MaxLength(48)]
    public string? ImageUrl { get; set; }

    [InverseProperty(nameof(SNFollowerModel.Follower))]
    public List<SNFollowerModel> Followers { get; set; } = [];

    [InverseProperty(nameof(SNFollowerModel.User))]
    public List<SNFollowerModel> Following { get; set; } = [];

    public List<ProjectTaskAssigneeModel> TaskAssignees { get; set; } = [];
}
