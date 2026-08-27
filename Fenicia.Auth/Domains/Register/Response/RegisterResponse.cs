using Fenicia.Auth.Domains.User.DTOs.Responses;

namespace Fenicia.Auth.Domains.Register.Response;

public record RegisterResponse(Guid Id, string Name, string Email, CreateNewUserCompanyResponse Company);
