namespace Fenicia.Auth.Domains.User.Commands;

public record UpdateUserCommand(Guid UserId, string? Name = null, string? Email = null, List<UpdateUserRoleCommand>? CompaniesRoles = null);