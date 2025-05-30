using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Contexts.Models;

[Table("companies")]
public class CompanyModel : BaseModel
{
    public string Name { get; set; } = null!;
    
    public virtual List<UserRoleModel> UsersRoles { get; set; } = [];
}