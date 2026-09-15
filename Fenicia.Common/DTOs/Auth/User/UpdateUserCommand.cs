using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.User;

public class UpdateUserCommand()
{
    public UpdateUserCommand(Guid userId, string? name = null, string? email = null, List<UpdateUserRoleCommand>? companiesRoles = null)
        : this()
    {
        UserId = userId;
        Name = name;
        Email = email;
        CompaniesRoles = companiesRoles;
    }

    public Guid UserId { get; set; }

    [StringLength(48)]
    public string? Name { get; set; }

    [EmailAddress]
    [StringLength(48)]
    public string? Email { get; set; }

    public List<UpdateUserRoleCommand>? CompaniesRoles { get; set; }
}
