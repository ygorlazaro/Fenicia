namespace Fenicia.Auth.Domains.User.ChangeUserPassword;

public record ChangeUserPasswordQuery(
    Guid UserId,
    string NewPassword
);