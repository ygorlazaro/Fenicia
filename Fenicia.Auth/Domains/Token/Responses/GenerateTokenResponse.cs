namespace Fenicia.Auth.Domains.Token.Responses;

public record GenerateTokenResponse(Guid Id, string Name, string Email);