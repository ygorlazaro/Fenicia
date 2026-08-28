namespace Fenicia.Auth.Domains.User.DTOs;

public record CreateNewUserResponse(Guid Id, string Name, string Email, CreateNewUserCompanyResponse Company);
