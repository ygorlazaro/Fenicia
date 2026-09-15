using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.User;

public class CreateUserRoleCommand()
{
    public CreateUserRoleCommand(Guid companyId, Guid roleId)
        : this()
    {
        CompanyId = companyId;
        RoleId = roleId;
    }

    [Required]
    public Guid CompanyId { get; set; }

    [Required]
    public Guid RoleId { get; set; }
}
