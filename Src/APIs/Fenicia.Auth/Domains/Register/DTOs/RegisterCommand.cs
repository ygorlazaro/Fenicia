using Fenicia.Auth.Domains.User.DTOs;

namespace Fenicia.Auth.Domains.Register.DTOs;

public record RegisterCommand(string Email, string Password, string Name, CreateNewUserCompanyCommand Company);
