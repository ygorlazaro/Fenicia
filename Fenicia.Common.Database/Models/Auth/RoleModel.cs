namespace Fenicia.Common.Database.Models.Auth;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Database;

[Table("roles")]
public class RoleModel : BaseModel
{
    #region Properties

    [Required(ErrorMessage = "Role name is required")]
    [MaxLength(length: 10, ErrorMessage = "Role name cannot exceed 10 characters")]
    [Column("name")]
    [Display(Name = "Role Name")]
    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual List<UserRoleModel> UsersRoles { get; set; } = [];

    #endregion
}
