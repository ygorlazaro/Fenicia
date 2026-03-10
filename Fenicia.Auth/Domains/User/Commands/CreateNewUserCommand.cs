namespace Fenicia.Auth.Domains.User.Commands;

public record CreateNewUserCommand(string Email, string Password, string Name, CreateNewUserCompanyCommand Company);