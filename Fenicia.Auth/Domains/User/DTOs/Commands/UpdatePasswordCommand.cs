using Fenicia.Auth.Domains.User.DTOs.Responses;


namespace Fenicia.Auth.Domains.User.DTOs.Commands;

public record UpdatePasswordCommand(Guid UserId, string Password);
