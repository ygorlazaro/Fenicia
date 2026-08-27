using Fenicia.Auth.Domains.Register.Response;
using Fenicia.Auth.Domains.User.DTOs.Commands;

namespace Fenicia.Auth.Domains.Register.Command;

public record RegisterCommand(string Email, string Password, string Name, CreateNewUserCompanyCommand Company);
