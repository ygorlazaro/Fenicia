namespace Fenicia.Auth.Domains.User.DTOs.Responses;

public record CreateNewUserResponse(Guid Id, string Name, string Email, CreateNewUserCompanyResponse Company);