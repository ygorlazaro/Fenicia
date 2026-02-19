namespace Fenicia.Auth.Domains.User.CreateNewUser;

public record CreateNewUserQuery(string Email, string Password, string Name, CreateNewUserCompanyQuery Company);