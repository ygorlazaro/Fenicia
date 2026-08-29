namespace Fenicia.Auth.Domains.Token.DTOs;

public record TokenResponse(string AccessToken, string RefreshToken, UserResponse User);
