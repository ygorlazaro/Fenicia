namespace Fenicia.Auth.Domains.User.Responses;

public record CreateNewUserResponse(Guid Id, string Name, string Email, CreateNewUserCompanyResponse Company);