namespace Fenicia.Auth.Domains.Role.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Common.Database;

using UserRole.Data;

[Table(name: "roles")]
public class RoleModel : BaseModel
{
    #region Properties

    [Required(ErrorMessage = "Role name is required")]
    [MaxLength(length: 10, ErrorMessage = "Role name cannot exceed 10 characters")]
    [Column(name: "name")]
    [Display(Name = "Role Name")]
    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual List<UserRoleModel> UsersRoles { get; set; } = [];

    #endregion
}
