namespace Fenicia.Auth.Domains.User.DTOs;

public record CreateNewUserCommand(string Email, string Password, string Name, CreateNewUserCompanyCommand Company);
