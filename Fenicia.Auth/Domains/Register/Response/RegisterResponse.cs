using Fenicia.Auth.Domains.User.Responses;

namespace Fenicia.Auth.Domains.Register.Response;

public record RegisterResponse(Guid Id, string Name, string Email, CreateNewUserCompanyResponse Company);
