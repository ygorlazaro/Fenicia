using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Role;

public class RoleRequest()
{
    public RoleRequest(Guid companyId, Guid roleId)
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
