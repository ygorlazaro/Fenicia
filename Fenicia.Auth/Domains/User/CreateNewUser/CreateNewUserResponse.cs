namespace Fenicia.Auth.Domains.User.CreateNewUser;

public record CreateNewUserResponse(Guid Id, string Name, string Email, CreateNewUserCompanyResponse Company);