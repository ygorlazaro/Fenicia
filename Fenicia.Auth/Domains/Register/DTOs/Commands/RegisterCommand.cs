using Fenicia.Auth.Domains.User.DTOs.Commands;

namespace Fenicia.Auth.Domains.Register.DTOs.Commands;

public record RegisterCommand(string Email, string Password, string Name, CreateNewUserCompanyCommand Company);
