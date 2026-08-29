using Fenicia.Auth.Domains.User.DTOs;

namespace Fenicia.Auth.Domains.Register.DTOs;

public record RegisterResponse(Guid Id, string Name, string Email, CreateNewUserCompanyResponse Company);
