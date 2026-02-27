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
    public List<AuthUserRole> UsersRoles { get; set; } = [];

    [JsonIgnore]
    public List<AuthOrder> Orders { get; set; } = [];

    [JsonIgnore]
    public List<ProjectTask> Tasks { get; set; } = [];

    [MaxLength(48)]
    public string? ImageUrl { get; set; }

    [InverseProperty(nameof(SNFollower.Follower))]
    public List<SNFollower> Followers { get; set; } = [];

    [InverseProperty(nameof(SNFollower.User))]
    public List<SNFollower> Following { get; set; } = [];
}
