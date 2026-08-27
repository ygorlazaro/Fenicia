namespace Fenicia.Auth.Domains.Token.DTOs.Responses;

public record GenerateTokenResponse(Guid Id, string Name, string Email);