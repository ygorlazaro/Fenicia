namespace Fenicia.Auth.Domains.User.DTOs.Commands;

public record CreateNewUserCommand(string Email, string Password, string Name, CreateNewUserCompanyCommand Company);
