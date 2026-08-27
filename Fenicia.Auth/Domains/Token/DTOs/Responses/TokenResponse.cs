namespace Fenicia.Auth.Domains.Token.DTOs.Responses;

public record TokenResponse(string AccessToken, string RefreshToken, UserResponse User);